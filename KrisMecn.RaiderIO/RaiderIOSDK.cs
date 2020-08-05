using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using KrisMecn.RaiderIO.Entities;
using KrisMecn.RaiderIO.Enums;
using KrisMecn.RaiderIO.Exceptions;
using KrisMecn.RaiderIO.Extensions;
using Newtonsoft.Json;

namespace KrisMecn.RaiderIO
{
    public class RaiderIOSDK : IDisposable
    {
        private readonly HttpClient _client;

        public RaiderIOSDK() : this(new Uri("https://raider.io/api/v1/")) { }

        public RaiderIOSDK(Uri baseAddress)
        {
            _client = new HttpClient()
            {
                BaseAddress = baseAddress,
                Timeout = TimeSpan.FromSeconds(10),
            };
        }

        /// <summary>
        /// Wrapper around <seealso cref="GetCharacterProfileRequest"/> for easier `fields` specification
        /// </summary>
        /// <param name="region">Region where the character is located</param>
        /// <param name="realm">Realm of the character. Can be full name or slug</param>
        /// <param name="characterName">Name of the character (case insensitive)</param>
        /// <returns>Request builder</returns>
        public CharacterProfileQueryBuilder GetCharacterProfile(Region region, string realm, string characterName)
        {
            return new CharacterProfileQueryBuilder(this, region, realm, characterName);
        }

        /// <summary>
        /// Get game character profile
        /// Use <seealso cref="GetCharacterProfile"/> for easier usage
        /// </summary>
        /// <param name="region">Region where the character is located</param>
        /// <param name="realm">Realm of the character. Can be full name or slug</param>
        /// <param name="characterName">Name of the character (case insensitive)</param>
        /// <param name="fields">Which optional fields to fetch</param>
        /// <returns></returns>
        public async Task<CharacterProfile> GetCharacterProfileRequest(Region region, string realm, string characterName, string fields)
        {
            var queryData = new Dictionary<string, string>() {
                {"region", region.ToEnumString()},
                {"realm", realm},
                {"name", characterName},
                {"fields", fields},
            };

            var response = await GetRequest($"characters/profile", queryData);
            return JsonConvert.DeserializeObject<CharacterProfile>(response);
        }

        /// <summary>
        /// Get currently active Mythic+ affixes
        /// </summary>
        /// <param name="region">Target game region</param>
        /// <param name="locale">Sets locale of the returned strings</param>
        public async Task<MythicPlusAffixes> GetMythicPlusAffixes(Region region, Locale locale = Locale.EN) {
            var queryData = new Dictionary<string, string>() {
                {"region", region.ToEnumString()},
                {"locale", locale.ToEnumString()},
            };

            var response = await GetRequest($"mythic-plus/affixes", queryData);
            return JsonConvert.DeserializeObject<MythicPlusAffixes>(response);
        }

        private async Task<string> GetRequest(string path, Dictionary<string, string> query) {
            string queryString;
            using (var data = new FormUrlEncodedContent(query))
            {
                queryString = await data.ReadAsStringAsync();
            }

            var res = await _client.GetAsync($"{path}?{queryString}");
            return await GetStringResponse(res);
        }

        private async Task<string> GetStringResponse(HttpResponseMessage message)
        {
            if (message == null)
            {
                throw new HttpRequestException("Empty response");
            }

            var response = await message.Content.ReadAsStringAsync();

            if ((int)message.StatusCode >= 500)
            {
                throw new HttpRequestException("Request ended in internal server error");
            }
            if ((int)message.StatusCode >= 400)
            {
                var errorMessage = JsonConvert.DeserializeObject<ErrorResponse>(response);

                throw new RaiderIOException(errorMessage?.Message ?? "Request failed");
            }

            return response;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
