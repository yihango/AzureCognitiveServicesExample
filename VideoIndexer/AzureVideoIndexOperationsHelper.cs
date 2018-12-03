using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace VideoIndexer
{
    public static class AzureVideoIndexOperationsHelper
    {
        // 官方api文档：https://api-portal.videoindexer.ai/docs/services/operations/


        /// <summary>
        /// 从url上传视频
        /// </summary>
        /// <param name="client"></param>
        /// <param name="videoUrl"></param>
        /// <param name="apiUrl"></param>
        /// <param name="location"></param>
        /// <param name="accountId"></param>
        /// <param name="accountAccessToken"></param>
        /// <returns></returns>
        public static string UploadVideoFromUrl(HttpClient client, string videoUrl, string apiUrl, string location, string accountId, string accountAccessToken)
        {
            // 上传文件
            var content = new MultipartFormDataContent();

            var uploadRequestResult = client.PostAsync($"{apiUrl}/{location}/Accounts/{accountId}/Videos?accessToken={accountAccessToken}&name=some_name&description=some_description&privacy=private&partition=some_partition&videoUrl={videoUrl}", content).Result;

            var uploadResult = uploadRequestResult.Content.ReadAsStringAsync().Result;

            return uploadResult;
        }

        /// <summary>
        /// 从文件上传视频
        /// </summary>
        /// <param name="client"></param>
        /// <param name="videoFilePath">视频文件路径</param>
        /// <param name="apiUrl"></param>
        /// <param name="location"></param>
        /// <param name="accountId"></param>
        /// <param name="accountAccessToken"></param>
        /// <returns></returns>
        public static string UploadVideoFromFile(HttpClient client, string videoFilePath, string apiUrl, string location, string accountId, string accountAccessToken)
        {
            // 上传文件
            var content = new MultipartFormDataContent();

            FileStream video = File.OpenRead(videoFilePath);
            byte[] buffer = new byte[video.Length];
            video.Read(buffer, 0, buffer.Length);
            content.Add(new ByteArrayContent(buffer));

            var uploadRequestResult = client.PostAsync($"{apiUrl}/{location}/Accounts/{accountId}/Videos?accessToken={accountAccessToken}&name=some_name&description=some_description&privacy=private&partition=some_partition", content).Result;

            var uploadResult = uploadRequestResult.Content.ReadAsStringAsync().Result;

            return uploadResult;
        }


        /// <summary>
        /// 获取视频索引状态
        /// </summary>
        /// <param name="client"></param>
        /// <param name="apiUrl"></param>
        /// <param name="location"></param>
        /// <param name="accountId"></param>
        /// <param name="videoId"></param>
        /// <param name="videoAccessToken"></param>
        /// <returns></returns>
        public static string GetVideoIndexState(HttpClient client, string apiUrl, string location, string accountId, string videoId, string videoAccessToken)
        {
            var videoGetIndexRequestResult = client.GetAsync($"{apiUrl}/{location}/Accounts/{accountId}/Videos/{videoId}/Index?accessToken={videoAccessToken}&language=English").Result;
            var videoGetIndexResult = videoGetIndexRequestResult.Content.ReadAsStringAsync().Result;

            return videoGetIndexResult;
        }


        /// <summary>
        /// 搜索视频
        /// </summary>
        /// <param name="client"></param>
        /// <param name="apiUrl"></param>
        /// <param name="location"></param>
        /// <param name="accountId"></param>
        /// <param name="accountAccessToken"></param>
        /// <param name="videoId"></param>
        /// <returns></returns>
        public static string SearchVideo(HttpClient client, string apiUrl, string location, string accountId, string accountAccessToken, string videoId)
        {
            var searchRequestResult = client.GetAsync($"{apiUrl}/{location}/Accounts/{accountId}/Videos/Search?accessToken={accountAccessToken}&id={videoId}").Result;
            var searchResult = searchRequestResult.Content.ReadAsStringAsync().Result;
            return searchResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="apiUrl"></param>
        /// <param name="location"></param>
        /// <param name="accountId"></param>
        /// <param name="accountAccessToken"></param>
        /// <param name="videoId"></param>
        /// <param name="videoAccessToken"></param>
        /// <returns></returns>
        public static Uri GetInsightsWidgetUrl(HttpClient client, string apiUrl, string location, string accountId, string accountAccessToken, string videoId, string videoAccessToken)
        {
            var insightsWidgetRequestResult = client.GetAsync($"{apiUrl}/{location}/Accounts/{accountId}/Videos/{videoId}/InsightsWidget?accessToken={videoAccessToken}&widgetType=Keywords&allowEdit=true").Result;
            var insightsWidgetLink = insightsWidgetRequestResult.Headers.Location;

            return insightsWidgetLink;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="apiUrl"></param>
        /// <param name="location"></param>
        /// <param name="accountId"></param>
        /// <param name="accountAccessToken"></param>
        /// <param name="videoId"></param>
        /// <param name="videoAccessToken"></param>
        /// <returns></returns>
        public static Uri GetPlayerWidgetUrl(HttpClient client, string apiUrl, string location, string accountId, string accountAccessToken, string videoId, string videoAccessToken)
        {
            var playerWidgetRequestResult = client.GetAsync($"{apiUrl}/{location}/Accounts/{accountId}/Videos/{videoId}/PlayerWidget?accessToken={videoAccessToken}").Result;
            var playerWidgetLink = playerWidgetRequestResult.Headers.Location;

            return playerWidgetLink;
        }
    }
}
