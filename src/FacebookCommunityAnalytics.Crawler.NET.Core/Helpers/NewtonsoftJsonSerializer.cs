using System.IO;
using Newtonsoft.Json;
using RS = RestSharp;

namespace FacebookCommunityAnalytics.Crawler.NET.Core.Helpers
{
    public class NewtonsoftJsonSerializer : RS.Serializers.ISerializer, RS.Deserializers.IDeserializer
    {
        private static NewtonsoftJsonSerializer _instance;
        private readonly JsonSerializer _serializer;

        public NewtonsoftJsonSerializer(JsonSerializer serializer)
        {
            _serializer = serializer;
        }

        public string ContentType
        {
            get { return "application/json"; } // Probably used for Serialization?
            set { }
        }

        public string DateFormat { get; set; }

        public string Namespace { get; set; }

        public string RootElement { get; set; }

        public string Serialize(object obj)
        {
            using var stringWriter = new StringWriter();
            using var jsonTextWriter = new JsonTextWriter(stringWriter);
            _serializer.Serialize(jsonTextWriter, obj);

            return stringWriter.ToString();
        }

        public T Deserialize<T>(RS.IRestResponse response)
        {
            var content = response.Content;

            using var stringReader = new StringReader(content);
            using var jsonTextReader = new JsonTextReader(stringReader);
            return _serializer.Deserialize<T>(jsonTextReader);
        }

        public static NewtonsoftJsonSerializer Default
        {
            get
            {
                return _instance ??= new NewtonsoftJsonSerializer(new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });
            }
        }
    }
}