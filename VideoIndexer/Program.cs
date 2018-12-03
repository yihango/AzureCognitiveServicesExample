using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace VideoIndexer
{
    class Program
    {
        static void Main(string[] args)
        {
            var apiUrl = "https://api.videoindexer.ai";

            // 账户类型,这是是试用
            var location = "trial";
            // 获取accountId: https://www.videoindexer.ai/settings/account
            var accountId = "";
            // 获取apiKey: https://api-portal.videoindexer.ai/developer
            var apiKey = "";

            System.Net.ServicePointManager.SecurityProtocol = System.Net.ServicePointManager.SecurityProtocol | System.Net.SecurityProtocolType.Tls12;

            // 创建HttpClient
            var handler = new HttpClientHandler();
            handler.AllowAutoRedirect = false;
            var client = new HttpClient(handler);



            // 获取 accountAccessToken
            var accountAccessToken = AzureVideoIndexAuthorizationHelper.GetAccountAccessToken(client, apiUrl, location, accountId, apiKey);


            #region 上传视频  从url上传视频 / 从文件上传视频

            Console.WriteLine("上传中...");

            var videoId = "";

            // 从url上传视频
            var videoUrl = "";
            var uploadResult = AzureVideoIndexOperationsHelper.UploadVideoFromUrl(client, videoUrl, apiUrl, location, accountId, accountAccessToken);

            // 从文件上传视频
            //var videoFilePath = "例如C:\\我的视频.mp4";
            //var uploadResult = AzureVideoIndexOperationsHelper.UploadVideoFromUrl(client, videoFilePath, apiUrl, location, accountId, accountAccessToken);

            // 从上传结果中获取视频id
            videoId = JsonConvert.DeserializeObject<dynamic>(uploadResult)["id"];
            Console.WriteLine($"上传结束!\r\n VideoID: {videoId}");

            #endregion



            // 获取视频访问令牌        
            var videoAccessToken = AzureVideoIndexAuthorizationHelper.GetVideoAccessToken(client, apiUrl, location, accountId, apiKey, videoId);



            //  等待视频索引完成
            while (true)
            {
                Thread.Sleep(10000);

                var videoGetIndexResult = AzureVideoIndexOperationsHelper.GetVideoIndexState(client, apiUrl, location, accountId, videoId, videoAccessToken);

                var processingState = JsonConvert.DeserializeObject<dynamic>(videoGetIndexResult)["state"];

                Console.WriteLine($"\r\n视频索引状态: {processingState}");


                // 判断视频索引任务是否已完成
                if (processingState != "Uploaded" && processingState != "Processing")
                {
                    Console.WriteLine($"\r\n 返回的json信息: {videoGetIndexResult}");
                    break;
                }
            }

            // 搜索视频
            var searchResult = AzureVideoIndexOperationsHelper.SearchVideo(client, apiUrl, location, accountId, accountAccessToken, videoId);
            Console.WriteLine($"\r\n搜索结果: {searchResult}");


            // 获取检索处理后地址 Insights
            var insightsWidgetLink = AzureVideoIndexOperationsHelper.GetInsightsWidgetUrl(client, apiUrl, location, accountId, accountAccessToken, videoId, videoAccessToken);
            Console.WriteLine($"\r\nInsights Widget url: {insightsWidgetLink}");



            // 获取播放地址 Player
            var playerWidgetLink = AzureVideoIndexOperationsHelper.GetPlayerWidgetUrl(client, apiUrl, location, accountId, accountAccessToken, videoId, videoAccessToken);
            Console.WriteLine($"\r\nPlayer Widget url: {playerWidgetLink}");


            Console.ReadKey();
        }
    }
}
