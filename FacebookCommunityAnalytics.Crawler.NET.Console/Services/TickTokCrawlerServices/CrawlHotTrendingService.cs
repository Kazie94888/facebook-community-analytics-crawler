using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FacebookCommunityAnalytics.Crawler.NET.Client.Clients;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Console.Services.TickTokCrawlerServices;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using Newtonsoft.Json;

namespace FacebookCommunityAnalytics.Crawler.NET.Console.Services.TickTokCrawlerServices
{
    public class CrawlHotTrendingService : BaseService
    {
        private const    string    Selector_AdbPathFile = @"D:\Program Files\Nox\bin\adb.exe";
        private readonly ApiClient _apiClient;
        
        public CrawlHotTrendingService(GlobalConfig globalConfig) : base(globalConfig)
        {
            _apiClient = new ApiClient(globalConfig.ApiConfig);
        }
        
        public void Execute()
        {
            IList<TrendingDetail> trendingDetails = new List<TrendingDetail>();

            ExecuteCommand("shell input tap 480 330");
            Wait(20000);
            ExecuteCommand("shell input tap 504 41");
            Wait(20000);
            ExecuteCommand("shell input text Trending");
            Wait(10000);
            ExecuteCommand("shell input tap 486 48");
            Wait(10000);
            
            var       hierarchy  = GetHierarchy();
            var firstCoordinate = GetCoordinateOfIndex(1, hierarchy.Node);
            // var secondCoordinate = GetCoordinateOfIndex(12, hierarchy.Node);

            var        index            = 1;
            Coordinate latestCoordinate = null;
            var       isReset          = false;
            while (true)
            {
                var coordinate = GetCoordinateOfIndex(index, hierarchy.Node);
                if (coordinate != null && coordinate.X != 0 && coordinate.Y != 0)
                {
                    latestCoordinate = coordinate;
                    if (isReset)
                    {
                        firstCoordinate = coordinate;
                        isReset         = false;
                    }
                    
                    var trendingDetail = GetDetails(index, hierarchy.Node); 
                    
                    System.Console.WriteLine(JsonConvert.SerializeObject(trendingDetail));
                    SaveTrendingDetail(trendingDetail);
                    
                    index += 1;
                }
                else
                {
                    if (isReset)
                    {
                        break;
                    }

                    if (latestCoordinate != null)
                    {
                        ExecuteCommand($"shell input swipe {latestCoordinate.X} {latestCoordinate.Y} {firstCoordinate.X} {firstCoordinate.Y} 1000");
                        Wait(20000);
                        hierarchy = GetHierarchy();
                    }

                    isReset = true;

                }
            }
            

            // for (var i = 1; i <= 12; i++)
            // {
            //     System.Console.WriteLine($"Process item {i}");
            //     var trendingDetail = GetDetails(i, hierarchy.Node);
            //
            //     SaveTrendingDetail(trendingDetail);
            // }
            
            // ExecuteCommand("shell input swipe 250 800 250 400 300");
            // ExecuteCommand($"shell input swipe {secondCoordinate.X} {secondCoordinate.Y} {firstCoordinate.X} {firstCoordinate.Y} 1000");
            // Wait(10000);
            // hierarchy = GetHierarchy();
            // firstCoordinate  = GetCoordinateOfIndex(13, hierarchy.Node);
            // secondCoordinate = GetCoordinateOfIndex(22, hierarchy.Node);
            //
            // // for (var i = 13; i <= 22; i++)
            // // {
            // //     var trendingDetail = GetDetails(i, hierarchy.Node); 
            // //     
            // //     SaveTrendingDetail(trendingDetail);
            // // }
            //
            // // ExecuteCommand("shell input swipe 250 800 250 400 300");
            // ExecuteCommand($"shell input swipe {secondCoordinate.X} {secondCoordinate.Y} {firstCoordinate.X} {firstCoordinate.Y} 1000");
            // Wait(10000);
            // hierarchy        = GetHierarchy();
            // firstCoordinate  = GetCoordinateOfIndex(23, hierarchy.Node);
            // secondCoordinate = GetCoordinateOfIndex(32, hierarchy.Node);
            //
            // // for (var i = 23; i <= 32; i++)
            // // {
            // //     var trendingDetail = GetDetails(i, hierarchy.Node); 
            // //     
            // //     SaveTrendingDetail(trendingDetail);
            // // }
            //
            // // ExecuteCommand("shell input swipe 250 800 250 400 300");
            // ExecuteCommand($"shell input swipe {secondCoordinate.X} {secondCoordinate.Y} {firstCoordinate.X} {firstCoordinate.Y} 1000");
            // Wait(10000);
            // hierarchy        = GetHierarchy();
            // firstCoordinate  = GetCoordinateOfIndex(33, hierarchy.Node);
            // secondCoordinate = GetCoordinateOfIndex(42, hierarchy.Node);
            //
            // // for (var i = 33; i <= 42; i++)
            // // {
            // //     var trendingDetail = GetDetails(i, hierarchy.Node); 
            // //     
            // //     SaveTrendingDetail(trendingDetail);
            // // }
            //
            // // ExecuteCommand("shell input swipe 250 800 250 400 300");
            // ExecuteCommand($"shell input swipe {secondCoordinate.X} {secondCoordinate.Y} {firstCoordinate.X} {firstCoordinate.Y} 1000");
            // Wait(10000);
            // hierarchy = GetHierarchy();
            
            // for (var i = 43; i <= 50; i++)
            // {
            //     var trendingDetail = GetDetails(i, hierarchy.Node); 
            //     SaveTrendingDetail(trendingDetail);
            // }

            ExecuteCommand("shell input keyevent KEYCODE_APP_SWITCH");
            Wait(2000);
            ExecuteCommand("shell input keyevent DEL");
            Wait(2000);

            // trendingDetails = trendingDetails.Where(detail => detail != null).ToList();
            // var tiktokTrendingDetailsDto = new List<TiktokTrendingDetailDto>();
            // foreach (var trendingDetail in trendingDetails)
            // {
            //     tiktokTrendingDetailsDto.Add(new TiktokTrendingDetailDto
            //     {
            //         Description = trendingDetail.Description,
            //         CreatedDateTime = DateTime.UtcNow,
            //         View = trendingDetail.View
            //     });
            // }
            //
            // if (tiktokTrendingDetailsDto.Any())
            // {
            //     _apiClient.TikTok.SaveTiktokTrending(new TiktokTrendingRequest
            //     {
            //         TrendingDetails = tiktokTrendingDetailsDto
            //     });
            // }
        }

        private void SaveTrendingDetail(TrendingDetail trendingDetail)
        {
            if (trendingDetail != null)
            {
                _apiClient.TikTok.SaveTiktokTrending(new TiktokTrendingRequest
                {
                    TrendingDetails = new List<TiktokTrendingDetailDto>
                    {
                        new() {Description = trendingDetail.Description, CreatedDateTime = DateTime.UtcNow, View = trendingDetail.View}
                    }
                });
            }
        }

        private Node GetNode(string textValue, Node node)
        {
            if (node.Text == textValue && node.Class == "com.bytedance.ies.xelement.LynxImpressionView")
            {
                return node;
            }

            if (node.SubNodes.Any())
            {
                foreach (var subNode in node.SubNodes)
                {
                    var node1 = GetNode(textValue, subNode);
                    if (node1 != null)
                    {
                        return node1;
                    }
                }
            }
            return null;
        }

        private Node GetDescriptionNode(Node node)
        {
            Node nodeValue = null;
            if (node.ResourceId.Contains("com.ss.android.ugc.trill:id") && node.Class == "X.0GH")
            {
                nodeValue = node;
            }
            
            if (nodeValue != null)
            {
                return PerformGettingDescriptionNode(nodeValue);
            }

            if (node.SubNodes.Any())
            {
                foreach (var subNode in node.SubNodes)
                {
                    var node1 = GetDescriptionNode(subNode);
                    if (node1 != null)
                    {
                        return node1;
                    }
                }
            }

            return null;
        }

        private Node GetViewNode(Node node)
        {
            Node nodeValue = null;
            if (node.ResourceId.Contains("com.ss.android.ugc.trill:id") && node.Class == "X.0GH")
            {
                nodeValue = node;
            }

            if (nodeValue != null)
            {
                return PerformGettingViewNode(nodeValue);
            }

            if (node.SubNodes.Any())
            {
                foreach (var subNode in node.SubNodes)
                {
                    var node1 = GetViewNode(subNode);
                    if (node1 != null)
                    {
                        return node1;
                    }
                }
            }

            return null;
        }

        private Node PerformGettingDescriptionNode(Node node)
        {
            if (node.Index == 10)
            {
                return node;
            }
            
            if (node.SubNodes.Any())
            {
                foreach (var subNode in node.SubNodes)
                {
                    var node1 = PerformGettingDescriptionNode(subNode);
                    if (node1 != null)
                    {
                        return node1;
                    }
                }
            }

            return null;
        }

        private Node PerformGettingViewNode(Node node)
        {
            if (node.Index == 7 && node.ResourceId != "com.ss.android.ugc.trill:id/fk_")
            {
                return node;
            }

            if (node.SubNodes.Any())
            {
                foreach (var subNode in node.SubNodes)
                {
                    var node1 = PerformGettingViewNode(subNode);
                    if (node1 != null)
                    {
                        return node1;
                    }
                }
            }

            return null;
        }

        private Coordinate GetCoordinateOfIndex(int i, Node node)
        {
            var nodeValue = GetNode(i.ToString(), node);
            if (nodeValue != null)
            {
                var bounds  = nodeValue.Bounds;
                var strings = bounds.Split("][");
                var coordinates = new Coordinates
                {
                    XY1 = new Coordinate
                    {
                        X = strings[0].Trim('[').Split(",")[0].ToDecimalOrDefault(),
                        Y = strings[0].Trim(']').Split(",")[1].ToDecimalOrDefault()
                    },
                    XY2 = new Coordinate
                    {
                        X = strings[1].Trim('[').Split(",")[0].ToDecimalOrDefault(),
                        Y = strings[1].Trim(']').Split(",")[1].ToDecimalOrDefault()
                    }
                };

                var coordinate = coordinates.GetCoordinate();

                return coordinate;
            }

            return null;
        }

        private TrendingDetail GetDetails(int i, Node node)
        {
            var nodeValue = GetNode(i.ToString(), node);
            if (nodeValue != null)
            {
                // bounds="[0,164][540,184]"
                var bounds = nodeValue.Bounds;
                var strings = bounds.Split("][");
                var coordinates = new Coordinates
                {
                    XY1 = new Coordinate
                    {
                        X = strings[0].Trim('[').Split(",")[0].ToDecimalOrDefault(),
                        Y = strings[0].Trim(']').Split(",")[1].ToDecimalOrDefault()
                    },
                    XY2 = new Coordinate
                    {
                        X = strings[1].Trim('[').Split(",")[0].ToDecimalOrDefault(),
                        Y = strings[1].Trim(']').Split(",")[1].ToDecimalOrDefault()
                    }
                };

                var coordinate = coordinates.GetCoordinate();
                System.Console.WriteLine($"shell input tap {coordinate.X} {coordinate.Y}");
                ExecuteCommand($"shell input tap {coordinate.X} {coordinate.Y}");
                Wait(20000);

                var hierarchy = GetHierarchy();
                var viewNode = GetViewNode(hierarchy.Node);
                if (viewNode == null)
                {
                    System.Console.WriteLine("View Node is Null");
                }
                var descriptionNode = GetDescriptionNode(hierarchy.Node);
                if (descriptionNode == null)
                {
                    System.Console.WriteLine("Description Node is Null");
                }

                ExecuteCommand("shell input tap 27 48.9");
                Wait(5000);
                
                System.Console.WriteLine($"View {viewNode.Text} - Description {descriptionNode.Text}");
                return new TrendingDetail(GlobalConfig, viewNode.Text, descriptionNode.Text);
            }

            return null;
        }

        private Hierarchy GetHierarchy()
        {
            ExecuteCommand("shell uiautomator dump");
            Wait(5000);
            
            if(File.Exists("window_dump.xml"))
            {
                System.Console.WriteLine("Delete Existing File");
                File.Delete("window_dump.xml");
            }

            ExecuteCommand("pull /sdcard/window_dump.xml window_dump.xml");
            Wait(2000);

            Hierarchy hierarchy;
            XmlSerializer serializer = new XmlSerializer(typeof(Hierarchy));
            using (StreamReader reader = new StreamReader("window_dump.xml"))
            {
                hierarchy = (Hierarchy) serializer.Deserialize(reader);
            }

            return hierarchy;
        }

        private void ExecuteCommand(string command)
        {
            using var myProcess = new Process();
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = Selector_AdbPathFile,
                Arguments = command
            };
            myProcess.StartInfo = startInfo;
            myProcess.Start();
        }

        private void Wait(int milliseconds)
        {
            using var autoResetEvent = new AutoResetEvent(false);
            autoResetEvent.WaitOne(milliseconds);
        }

        
    }
}

    [XmlRoot(ElementName="node")]
    public class Node { 

        [XmlElement(ElementName="node")] 
        public List<Node> SubNodes { get; set; } 

        [XmlAttribute(AttributeName="index")] 
        public int Index { get; set; } 

        [XmlAttribute(AttributeName="text")] 
        public string Text { get; set; } 

        [XmlAttribute(AttributeName="resource-id")] 
        public string ResourceId { get; set; } 

        [XmlAttribute(AttributeName="class")] 
        public string Class { get; set; } 

        [XmlAttribute(AttributeName="package")] 
        public string Package { get; set; } 

        [XmlAttribute(AttributeName="content-desc")] 
        public string ContentDesc { get; set; } 

        [XmlAttribute(AttributeName="checkable")] 
        public bool Checkable { get; set; } 

        [XmlAttribute(AttributeName="checked")] 
        public bool Checked { get; set; } 

        [XmlAttribute(AttributeName="clickable")] 
        public bool Clickable { get; set; } 

        [XmlAttribute(AttributeName="enabled")] 
        public bool Enabled { get; set; } 

        [XmlAttribute(AttributeName="focusable")] 
        public bool Focusable { get; set; } 

        [XmlAttribute(AttributeName="focused")] 
        public bool Focused { get; set; } 

        [XmlAttribute(AttributeName="scrollable")] 
        public bool Scrollable { get; set; } 

        [XmlAttribute(AttributeName="long-clickable")] 
        public bool LongClickable { get; set; } 

        [XmlAttribute(AttributeName="password")] 
        public bool Password { get; set; } 

        [XmlAttribute(AttributeName="selected")] 
        public bool Selected { get; set; } 

        [XmlAttribute(AttributeName="bounds")] 
        public string Bounds { get; set; } 

        [XmlAttribute(AttributeName="NAF")] 
        public bool NAF { get; set; } 
    }

    [XmlRoot(ElementName="hierarchy")]
    public class Hierarchy { 

        [XmlElement(ElementName="node")] 
        public Node Node { get; set; } 

        [XmlAttribute(AttributeName="rotation")] 
        public int Rotation { get; set; } 
    }

    public class Coordinates
    {
        public Coordinate XY1 { get; set; }
        public Coordinate XY2 { get; set; }


        public Coordinate GetCoordinate()
        {
            return new Coordinate
            {
                X = (XY1.X + XY2.X) / 2,
                Y = (XY1.Y + XY2.Y) / 2
            };
        }
    }

    public class Coordinate
    {
        public decimal X { get; set; }
        public decimal Y { get; set; }
    }

    public class TrendingDetail : BaseService
    {
        
        public int View { get; private set; }
        public string Description { get; private set; }

        public TrendingDetail(GlobalConfig globalConfig, string view, string description) : base(globalConfig)
        {
            Description = description;
            view = view.Replace("lượt xem", "").Trim();
            View = ConvertToTotalCount(view);
        }

    }

