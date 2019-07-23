using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Wordplay.Words
{
    class WordsAPI
    {
        private HttpClient _client;
        private Uri uri;

        public WordsAPI()
        {
            _client = new HttpClient();

            // save path to post to
            uri = new Uri("https://scrabble.hasbro.com/en-us/tools#dictionary");
        }

        public async Task<bool> DoesWordExist(string word)
        {
            // create content to send over
            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("dictWord", word));
            HttpContent content = new FormUrlEncodedContent(keyValues);

            // execute request to scrabble.hasbro.com
            bool isWord = await GetResponseIsWordAsync(uri, content);

            return isWord;
        }

        private async Task<bool> GetResponseIsWordAsync(Uri uri, HttpContent content)
        {
            HttpResponseMessage res = await _client.PostAsync(uri, content);
            if (res.IsSuccessStatusCode)
            {
                var response = await res.Content.ReadAsStringAsync();   // get the html responseText
                return response.Contains("CONGRATULATIONS!");   // is an official Scrabble word
            }
            return false;
        }
    }
}
