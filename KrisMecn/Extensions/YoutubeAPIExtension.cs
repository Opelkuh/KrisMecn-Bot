using DSharpPlus;
using System.Threading.Tasks;
using System.Collections.Generic;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace KrisMecn.Extensions
{
    class YoutubeAPIExtension : BaseExtension
    {
        public YouTubeService YoutubeService { get; private set; }

        public YoutubeAPIExtension(string apiKey) : base()
        {
            YoutubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey
            });
        }

        public async Task<IList<SearchResult>> Search(string query)
        {
            var searchReq = YoutubeService.Search.List("snippet");

            searchReq.Q = query;
            searchReq.MaxResults = 5;
            searchReq.Type = "video";
            searchReq.SafeSearch = SearchResource.ListRequest.SafeSearchEnum.None;

            var res = await searchReq.ExecuteAsync();

            return res.Items;
        }

        protected override void Setup(DiscordClient client)
        {
        }
    }
}
