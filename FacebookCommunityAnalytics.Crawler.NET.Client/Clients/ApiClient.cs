using System;
using System.Security.Authentication;
using FacebookCommunityAnalytics.Crawler.NET.Client.Configurations;
using FacebookCommunityAnalytics.Crawler.NET.Client.Consumers;
using FacebookCommunityAnalytics.Crawler.NET.Client.Entities;
using FacebookCommunityAnalytics.Crawler.NET.Core.Extensions;
using FacebookCommunityAnalytics.Crawler.NET.Core.Helpers;
using RestSharp;

namespace FacebookCommunityAnalytics.Crawler.NET.Client.Clients
{
    public interface IApiClient
    {
        
    }
    
    public class ApiClient
    {
        private readonly RestClient _client;
        private readonly RestClient _authClient;
        private readonly RestClient _viotpClient;

        private TokenModel.Response _accessToken;

        private const string AuthError = "Please authenticate before attempting to use the API";
        
        private string _apiUrl;
        private string _authUrl;
        private string _viotpUrl;

        public ApiClient(ApiConfig config)
        {
            _apiUrl = config.ApiUrl;
            _viotpUrl = config.ViotpUrl;
            
// #if DEBUG
//             _apiUrl = "https://localhost:44344/api";
// #endif

            
            _client = RestHelper.CreateClient(_apiUrl);
            _viotpClient = RestHelper.CreateClient(_viotpUrl);
            
            if (!string.IsNullOrWhiteSpace(config.AuthUrl))
            {
                _authUrl = config.AuthUrl;
// #if DEBUG
//                 _authUrl = "https://localhost:44382";
// #endif
                _authClient = RestHelper.CreateClient(_authUrl);
                _auth = new AuthApiConsumer(_authClient);
                Auth();
            }

            _crawl = new CrawlApiConsumer(_client);
            _group = new GroupApiConsumer(_client);
            _scheduledPostApiConsumer = new ScheduledPostApiConsumer(_client);
            _account = new AccountApiConsumer(_client);
            _tiktokApiConsumer = new TiktokApiConsumer(_client);
            _viotpConsumer = new ViotpConsumer(_viotpClient);
            _anonymousConsumer = new AnonymousConsumer(_client);
        }
        
        // private string _baseUrl;
        // public ApiClient(string baseUrl)
        // {
        //     _baseUrl = baseUrl;
        //     _client = RestHelper.CreateClient(baseUrl);
        // }

        private readonly AuthApiConsumer _auth;
        private readonly CrawlApiConsumer _crawl;
        private readonly ScheduledPostApiConsumer _scheduledPostApiConsumer;
        private readonly GroupApiConsumer _group;
        private readonly AccountApiConsumer _account;
        private readonly TiktokApiConsumer _tiktokApiConsumer;
        private readonly ViotpConsumer _viotpConsumer;
        private readonly AnonymousConsumer _anonymousConsumer;

        private void Auth()
        {
            var tokenResponse = _auth.PostTokenDefault();
            if (tokenResponse.IsSuccess)
            {
                _accessToken = tokenResponse.Resource;
                SetAuthHeader(_accessToken.access_token);
            }
        }
        
        private void SetAuthHeader(string token)
        {
            try
            {
                // lib exception: sequence contains more than 1 element
                _client.RemoveDefaultParameter("Authorization");
            }
            catch (Exception)
            {
                // ignored
            }

            _client.AddDefaultHeader("Authorization", "Bearer {0}".FormatWith(token));
        }

        public CrawlApiConsumer Crawl
        {
            get
            {
                if (_accessToken == null && !string.IsNullOrWhiteSpace(_authUrl))
                {
                    throw new AuthenticationException(AuthError);
                }

                return _crawl;
            }
        }

        public TiktokApiConsumer TikTok
        {
            get
            {
                if (_accessToken == null && !string.IsNullOrWhiteSpace(_authUrl))
                {
                    throw new AuthenticationException(AuthError);
                }

                return _tiktokApiConsumer;
            }
        }

        public ScheduledPostApiConsumer ScheduledPostApiConsumer
        {
            get
            {
                if (_accessToken == null && !string.IsNullOrWhiteSpace(_authUrl))
                {
                    throw new AuthenticationException(AuthError);
                }

                return _scheduledPostApiConsumer;
            }
        }
        
        public GroupApiConsumer Group
        {
            get
            {
                if (_accessToken == null && !string.IsNullOrWhiteSpace(_authUrl))
                {
                    throw new AuthenticationException(AuthError);
                }

                return _group;
            }
        }
        
        public AccountApiConsumer Account
        {
            get
            {
                if (_accessToken == null && !string.IsNullOrWhiteSpace(_authUrl))
                {
                    throw new AuthenticationException(AuthError);
                }

                return _account;
            }
        }

        public ViotpConsumer Viotp
        {
            get
            {
                return _viotpConsumer;
            }
        }

        public AnonymousConsumer Anonymous
        {
            get
            {
                return _anonymousConsumer;
            }
        }
        
    }
}