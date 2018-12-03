using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace VideoIndexer
{
    public static class AzureVideoIndexAuthorizationHelper
    {
        // 官方api文档: https://api-portal.videoindexer.ai/docs/services/authorization/


        /// <summary>
        /// 获取所有的账户的信息以及Token
        /// </summary>
        /// <param name="client"></param>
        /// <param name="apiUrl"></param>
        /// <param name="location"></param>
        /// <param name="apiKey"></param>
        /// <param name="generateAccessTokens">是否为每个帐户生成访问令牌。</param>
        /// <param name="allowEdit">令牌是否具有写权限</param>
        /// <returns></returns>
        public static string GetAccounts(HttpClient client, string apiUrl, string location, string apiKey, bool generateAccessTokens = true, bool allowEdit = false)
        {
            // HttpClient附加apiKey
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

            var generateAccessTokensStr = generateAccessTokens ? "true" : "false";
            var allowEditStr = allowEdit ? "true" : "false";

            var response = client.GetAsync($"{apiUrl}/auth/{location}/Accounts?generateAccessTokens={generateAccessTokensStr}&allowEdit={allowEditStr}").Result;

            var result = response.Content.ReadAsStringAsync().Result.Replace("\"", "");


            client.DefaultRequestHeaders.Remove("Ocp-Apim-Subscription-Key");

            return result;
        }


        /// <summary>
        /// 获取某个账户的AccessToken
        /// </summary>
        /// <param name="client"></param>
        /// <param name="apiUrl"></param>
        /// <param name="location"></param>
        /// <param name="accountId"></param>'
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public static string GetAccountAccessToken(HttpClient client, string apiUrl, string location, string accountId, string apiKey)
        {
            // HttpClient附加apiKey
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

            // 获取token
            var accountAccessTokenRequestResult = client.GetAsync($"{apiUrl}/auth/{location}/Accounts/{accountId}/AccessToken?allowEdit=true").Result;
            var accountAccessToken = accountAccessTokenRequestResult.Content.ReadAsStringAsync().Result.Replace("\"", "");

            client.DefaultRequestHeaders.Remove("Ocp-Apim-Subscription-Key");

            return accountAccessToken;
        }

        /// <summary>
        /// 获取VideoAccessToken
        /// </summary>
        /// <param name="client"></param>
        /// <param name="apiUrl"></param>
        /// <param name="location"></param>
        /// <param name="accountId"></param>
        /// <param name="apiKey"></param>
        /// <param name="videoId">视频Id</param>
        /// <returns></returns>
        public static string GetVideoAccessToken(HttpClient client, string apiUrl, string location, string accountId, string apiKey, string videoId)
        {
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
            var videoTokenRequestResult = client.GetAsync($"{apiUrl}/auth/{location}/Accounts/{accountId}/Videos/{videoId}/AccessToken?allowEdit=true").Result;
            var videoAccessToken = videoTokenRequestResult.Content.ReadAsStringAsync().Result.Replace("\"", "");

            client.DefaultRequestHeaders.Remove("Ocp-Apim-Subscription-Key");

            return videoAccessToken;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="apiUrl"></param>
        /// <param name="location"></param>
        /// <param name="apiKey"></param>
        /// <param name="allowEdit"></param>
        /// <returns></returns>
        public static string GetUserAccessToken(HttpClient client, string apiUrl, string location, string apiKey, bool allowEdit = false)
        {
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
            var response = client.GetAsync($"{apiUrl}/auth/{location}/Users/Me/AccessToken?allowEdit={allowEdit.ToString()}").Result;
            var result = response.Content.ReadAsStringAsync().Result.Replace("\"", "");

            client.DefaultRequestHeaders.Remove("Ocp-Apim-Subscription-Key");

            return result;
        }
    }
}
