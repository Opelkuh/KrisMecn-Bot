using BooruSharp.Booru;
using BooruSharp.Search.Post;
using DSharpPlus;
using System;
using System.Threading.Tasks;

namespace KrisMecn.Extensions
{
    class BooruExtension : BaseExtension
    {
        private Rule34 _r34 = new Rule34();
        private DanbooruDonmai _danbooru = new DanbooruDonmai();
        private Gelbooru _gelbooru = new Gelbooru();
        private Safebooru _safebooru = new Safebooru();
        private SankakuComplex _sankakuComplex = new SankakuComplex();

        private ABooru GetClientFromSite(BooruSite site)
        {
            return site switch
            {
                BooruSite.Rule34 => _r34,
                BooruSite.Danbooru => _danbooru,
                BooruSite.Gelbooru => _gelbooru,
                BooruSite.Safebooru => _safebooru,
                BooruSite.SankakuComplex => _sankakuComplex,
                _ => throw new NotImplementedException(),
            };
        }

        public string GetSiteName(BooruSite site)
        {
            return site switch
            {
                BooruSite.Rule34 => "Rule34.xxx",
                BooruSite.Danbooru => "Danbooru",
                BooruSite.Gelbooru => "Gelbooru",
                BooruSite.Safebooru => "Safebooru",
                BooruSite.SankakuComplex => "Sankaku Complex",
                _ => throw new NotImplementedException(),
            };
        }

        public async Task<SearchResult?> GetRandomImage(BooruSite site, string tags)
        {
            var client = GetClientFromSite(site);

            try
            {
                return await client.GetRandomPostAsync(tags);
            }
            catch (Exception e)
            {
                Logger.Error(e);

                return null;
            }
        }
    }

    enum BooruSite
    {
        Rule34,
        Danbooru,
        Gelbooru,
        Safebooru,
        SankakuComplex,
    }
}