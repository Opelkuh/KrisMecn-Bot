using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Text;
using BooruSharp.Booru;
using System.Threading.Tasks;
using BooruSharp.Search.Post;

namespace KrisMecn.Extensions
{
    class BooruExtension : BaseExtension
    {
        private Rule34 _r34 = new Rule34();
        private DanbooruDonmai _danbooru = new DanbooruDonmai();
        private Gelbooru _gelbooru = new Gelbooru();
        private Safebooru _safebooru = new Safebooru();
        private SankakuComplex _sankakuComplex = new SankakuComplex();

        public BooruExtension()
        {
        }

        protected override void Setup(DiscordClient client)
        {
        }

        private ABooru GetClientFromSite(BooruSite site)
        {
            return site switch
            {
                BooruSite.Rule34 => _r34,
                BooruSite.Danbooru => _danbooru,
                BooruSite.Gelbooru => _gelbooru,
                BooruSite.Safebooru => _safebooru,
                BooruSite.SankakuChannel => _sankakuComplex,
                _ => throw new NotImplementedException(),
            };
        }

        public Task<SearchResult> GetRandomImage(BooruSite site, string tags)
        {
            var client = GetClientFromSite(site);

            return client.GetRandomImageAsync(tags);
        }
    }

    enum BooruSite
    {
        Rule34,
        Danbooru,
        Gelbooru,
        Safebooru,
        SankakuChannel,
    }
}