using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Client.Enums;
using Microsoft.Playwright;
using TL;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.CrawlerServices
{
    public class CrawlTelegramService : CrawlServiceBase
    {
        private static WTelegram.Client _client;
        private static User _my;
        private static readonly Dictionary<long, User> Users = new();
        private static readonly Dictionary<long, ChatBase> Chats = new();
        private static List<TelegramResult> _telegramResults = new List<TelegramResult>();
        private static object _locked = new object();


        private static void Client_Update(IObject arg)
        {
            if (arg is not UpdatesBase updates) return;
            updates.CollectUsersChats(Users, Chats);
            foreach (var update in updates.UpdateList)
                switch (update)
                {
                    case UpdateNewMessage unm:  DisplayMessage(unm.message); break;
                    case UpdateEditMessage uem: DisplayMessage(uem.message, true); break;
                    case UpdateDeleteChannelMessages udcm:
                    {
                        foreach (var udmMessage in udcm.messages)
                        {
                            if (_telegramResults != null)
                            {
                                var telegramResult = _telegramResults.FirstOrDefault(result => result.MessageId == udmMessage);
                                if (telegramResult != null)
                                {
                                    _telegramResults.Remove(telegramResult);
                                }
                            }
                        }
                        System.Console.WriteLine($"{udcm.messages.Length} message(s) deleted in {Chat(udcm.channel_id)}"); 
                        break;
                    }
                    case UpdateDeleteMessages udm:
                    {
                        foreach (var udmMessage in udm.messages)
                        {
                            if (_telegramResults != null)
                            {
                                var telegramResult = _telegramResults.FirstOrDefault(result => result.MessageId == udmMessage);
                                if (telegramResult != null)
                                {
                                    _telegramResults.Remove(telegramResult);
                                }
                            }
                        }
                        
                        System.Console.WriteLine($"{udm.messages.Length} message(s) deleted"); 
                        break;
                    }
                    case UpdateUserTyping uut: System.Console.WriteLine($"{User(uut.user_id)} is {uut.action}"); break;
                    case UpdateChatUserTyping ucut: System.Console.WriteLine($"{Peer(ucut.from_id)} is {ucut.action} in {Chat(ucut.chat_id)}"); break;
                    case UpdateChannelUserTyping ucut2: System.Console.WriteLine($"{Peer(ucut2.from_id)} is {ucut2.action} in {Chat(ucut2.channel_id)}"); break;
                    case UpdateChatParticipants { participants: ChatParticipants cp }: System.Console.WriteLine($"{cp.participants.Length} participants in {Chat(cp.chat_id)}"); break;
                    case UpdateUserStatus uus: System.Console.WriteLine($"{User(uus.user_id)} is now {uus.status.GetType().Name[10..]}"); break;
                    case UpdateUserName uun: System.Console.WriteLine($"{User(uun.user_id)} has changed profile name: @{uun.username} {uun.first_name} {uun.last_name}"); break;
                    case UpdateUserPhoto uup: System.Console.WriteLine($"{User(uup.user_id)} has changed profile photo"); break;
                    default: System.Console.WriteLine(update.GetType().Name); break; // there are much more update types than the above cases
                }
        }
        
        private static async Task DisplayMessage(MessageBase messageBase, bool edit = false)
        {
            if (edit) System.Console.Write("(Edit): ");
            
            switch (messageBase)
            {
                case Message m:
                {
                    string htmlContent = string.Empty;
                    if (m.entities != null && m.entities.Any())
                    {
                        htmlContent = _client.EntitiesToHtml(m.message, m.entities);
                    }

                    var fileBase64String = string.Empty;
                    var fileName = string.Empty;
                    if (m.media is MessageMediaPhoto {photo: Photo photo})
                    {
                        fileName = $"{photo.id}.jpg";
                        System.Console.WriteLine("Downloading " + fileName);
                        await using var fileStream = File.Create(fileName);
                        var type = await _client.DownloadFileAsync(photo, fileStream);
                        fileStream.Close(); // necessary for the renaming
                        System.Console.WriteLine("Download finished");
                        
                        if (type is not Storage_FileType.unknown and not Storage_FileType.partial)
                            File.Move(fileName, $"{photo.id}.{type}", true);
                        
                        var bytes = await File.ReadAllBytesAsync(fileName);
                        fileBase64String = Convert.ToBase64String(bytes);
                    }
                    // else if (m.media is MessageMediaDocument { document: Document document })
                    // {
                    //     int slash = document.mime_type.IndexOf('/'); // quick & dirty conversion from MIME type to file extension
                    //     var filename = slash > 0 ? $"{document.id}.{document.mime_type[(slash + 1)..]}" : $"{document.id}.bin";
                    //     System.Console.WriteLine("Downloading " + filename);
                    //     await using var fileStream = File.Create(filename);
                    //     await _client.DownloadFileAsync(document, fileStream);
                    //     System.Console.WriteLine("Download finished");
                    // }
                    

                    lock (_locked)
                    {
                        if (m.grouped_id != 0)
                        {
                            var telegramResult =
                                _telegramResults.FirstOrDefault(result => result.GroupId == m.grouped_id);
                            if (telegramResult != null)
                            {
                                if (!string.IsNullOrWhiteSpace(fileName))
                                {
                                    if (telegramResult.Images == null || !telegramResult.Images.Any())
                                    {
                                        telegramResult.Images = new Dictionary<string, string>
                                            {{fileName, fileBase64String}};
                                    }
                                    else
                                    {
                                        if (telegramResult.Images.Keys.Contains(fileName))
                                        {
                                            telegramResult.Images[fileName] = fileBase64String;
                                        }
                                        else
                                        {
                                            telegramResult.Images.Add(fileName, fileBase64String);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                telegramResult = new TelegramResult();
                                if (!string.IsNullOrWhiteSpace(fileName))
                                {
                                    telegramResult.Images = new Dictionary<string, string> {{fileName, fileBase64String}};
                                }

                                _telegramResults.Add(telegramResult);
                            }

                            telegramResult.Content = !string.IsNullOrWhiteSpace(htmlContent) ? htmlContent : m.message;
                            telegramResult.ChannelId = m.Peer.ID;
                            telegramResult.DateTime = m.Date;
                            telegramResult.MessageId = m.ID;
                            telegramResult.GroupId = m.grouped_id;
                            
                            System.Console.WriteLine($"=======================Channel {telegramResult.ChannelId}====================================");
                            System.Console.WriteLine($" Content {telegramResult.Content} - DateTime {telegramResult.DateTime} - MessageId {telegramResult.MessageId} - GroupId {telegramResult.GroupId}");
                            System.Console.WriteLine($"=================================================================================");

                        }
                        else
                        {
                            var telegramResult = _telegramResults.FirstOrDefault(result =>
                                result.MessageId == m.id && result.ChannelId == m.Peer.ID);
                            if (telegramResult != null)
                            {
                                if (!string.IsNullOrWhiteSpace(fileName))
                                {
                                    if (telegramResult.Images == null || !telegramResult.Images.Any())
                                    {
                                        telegramResult.Images = new Dictionary<string, string>
                                            {{fileName, fileBase64String}};
                                    }
                                    else
                                    {
                                        if (telegramResult.Images.Keys.Contains(fileName))
                                        {
                                            telegramResult.Images[fileName] = fileBase64String;
                                        }
                                        else
                                        {
                                            telegramResult.Images.Add(fileName, fileBase64String);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                telegramResult = new TelegramResult();
                                if (!string.IsNullOrWhiteSpace(fileName))
                                {
                                    telegramResult.Images = new Dictionary<string, string> {{fileName, fileBase64String}};
                                }

                                _telegramResults.Add(telegramResult);
                            }
                            
                            telegramResult.Content = !string.IsNullOrWhiteSpace(htmlContent) ? htmlContent : m.message;
                            telegramResult.ChannelId = m.Peer.ID;
                            telegramResult.DateTime = m.Date;
                            telegramResult.MessageId = m.ID;
                            telegramResult.GroupId = m.grouped_id;
                            
                            System.Console.WriteLine($"=======================Channel {telegramResult.ChannelId}====================================");
                            System.Console.WriteLine($" Content {telegramResult.Content} - DateTime {telegramResult.DateTime} - MessageId {telegramResult.MessageId} - GroupId {telegramResult.GroupId}");
                            System.Console.WriteLine($"=================================================================================");
                        }
                    }

                    
                    // System.Console.WriteLine($"{Peer(m.from_id) ?? m.post_author} in {Peer(m.peer_id)}> {m.message}");
                    
                    break;
                }
                case MessageService ms:
                {
                    System.Console.WriteLine($"{Peer(ms.from_id)} in {Peer(ms.peer_id)} [{ms.action.GetType().Name[13..]}]"); break;
                }
            }
        }

        private static string User(long id) => Users.TryGetValue(id, out var user) ? user.ToString() : $"User {id}";
        private static string Chat(long id) => Chats.TryGetValue(id, out var chat) ? chat.ToString() : $"Chat {id}";
        private static string Peer(Peer peer) => peer is null ? null : peer is PeerUser user ? User(user.user_id)
            : peer is PeerChat or PeerChannel ? Chat(peer.ID) : $"Peer {peer.ID}";

        public CrawlTelegramService(GlobalConfig globalConfig) : base(globalConfig)
        {
        }

        protected override AccountType AccountType { get; }
        protected override CrawlStopCondition CrawlStopCondition { get; }
        public override async Task Execute()
        {
            Environment.SetEnvironmentVariable("api_hash", "bf88ba1c34f7cf3c8ab52c6f347dbc42");
            Environment.SetEnvironmentVariable("api_id", "13716956");
            Environment.SetEnvironmentVariable("phone_number", "+84935331834");
            
            System.Console.WriteLine("The program will display updates received for the logged-in user. Press any key to terminate");
            
            _client = new WTelegram.Client(Environment.GetEnvironmentVariable)
            {
                MaxAutoReconnects = 1000,
                PingInterval = 30
            };
            
            
            using (_client)
            {
                _client.Update += Client_Update;
                _my = await _client.LoginUserIfNeeded();
                Users[_my.id] = _my;
                // Note that on login Telegram may sends a bunch of updates/messages that happened in the past and were not acknowledged
                System.Console.WriteLine($"We are logged-in as {_my.username ?? _my.first_name + " " + _my.last_name} (id {_my.id})");
                // We collect all infos about the users/chats so that updates can be printed with their names
                var dialogs = await _client.Messages_GetAllDialogs(); // dialogs = groups/channels/users
                dialogs.CollectUsersChats(Users, Chats);
                System.Console.ReadKey();
            }
        }

        protected override Task<CrawlResult> CanCrawl(IPage page, CrawlModelBase crawlItem)
        {
            throw new System.NotImplementedException();
        }

        protected override Task<CrawlResult> DoCrawl(IPage page, CrawlModelBase crawlItem)
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class TelegramResult
    {
        /// <summary>
        /// Group Id is used for the post that have a lot of image
        /// </summary>
        public long GroupId { get; set; }
        public long MessageId { get; set; }
        public long ChannelId { get; set; }
        public DateTime? DateTime { get; set; }
        public string Content { get; set; }
        public Dictionary<string, string> Images { get; set; }
    }
}