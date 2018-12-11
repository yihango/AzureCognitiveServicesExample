using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TextToSpeech
{
    class Program
    {
        // Azure文字转语言,官方文档地址: https://docs.microsoft.com/zh-cn/azure/cognitive-services/speech-service/


        static string apiKey = "";// azure订阅key
        static string resouceName = "ai-azure-2018";// azure中的资源名称

        // 地区 (westus/eastasia/...)
        static string region = "eastasia";
        // 认证端点
        static string endpoint = " https://{0}.api.cognitive.microsoft.com/sts/v1.0/issuetoken";
        // 服务
        static string host = "https://{0}.tts.speech.microsoft.com/cognitiveservices/v1";
        // 数据模板
        // 语言支持详见:https://docs.microsoft.com/zh-cn/azure/cognitive-services/speech-service/language-support
        // 更多语言合成配置详见:https://docs.microsoft.com/zh-cn/azure/cognitive-services/speech-service/speech-synthesis-markup
        static string bodyTemplate = @"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='{0}'>
              <voice name='Microsoft Server Speech Text to Speech Voice ({1})'>{2}</voice></speak>";


        static void Main(string[] args)
        {
            endpoint = string.Format(endpoint, region);
            host = string.Format(host, region);


            try
            {
                Run().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.ReadKey();
        }


        static async Task Run()
        {

            //

            //Console.Write("请输入你想转换成语音的内容: ");
            //string text = Console.ReadLine();


            var filePath = @"测试文本内容.txt";
            var text = await File.ReadAllTextAsync(filePath, Encoding.UTF8);


            Console.WriteLine("正在获取token..请稍后..\n");

            string accessToken;
            Authentication auth = new Authentication(endpoint, apiKey);
            try
            {
                accessToken = await auth.FetchTokenAsync().ConfigureAwait(false);
                Console.WriteLine("已成功获取token. \n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取访问Token失败。.");
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.Message);
                return;
            }




            var body = string.Empty;
            //body = string.Format(bodyTemplate, "zh-CN", "zh-CN, HuihuiRUS", text);// 汉语-普通话
            //body = string.Format(bodyTemplate, "zh-HK", "zh-HK, Tracy, Apollo", text);// 汉语-香港
            body = string.Format(bodyTemplate, "zh-TW", "zh-TW, Yating, Apollo", text);// 汉语-台湾
            //body = string.Format(bodyTemplate, "en-US", "en-US, ZiraRUS", text);// 英语

            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    // 设置请求方式为post
                    request.Method = HttpMethod.Post;
                    // 设置url
                    request.RequestUri = new Uri(host);
                    // 设置content类型
                    request.Content = new StringContent(body, Encoding.UTF8, "application/ssml+xml");
                    // 设置token
                    request.Headers.Add("Authorization", "Bearer " + accessToken);
                    request.Headers.Add("Connection", "Keep-Alive");
                    // 设置资源名称
                    request.Headers.Add("User-Agent", resouceName);
                    request.Headers.Add("X-Microsoft-OutputFormat", "riff-16khz-16bit-mono-pcm");
                    //request.Headers.Add("X-Microsoft-OutputFormat", "riff-24khz-16bit-mono-pcm");
                    // 创建请求
                    Console.WriteLine("正在调用Azure TTS 服务,请稍后.. \n");
                    using (var response = await client.SendAsync(request).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        // 异步读取响应
                        using (var dataStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            Console.WriteLine("正在将数据写入音频文件...");
                            using (var fileStream = new FileStream(@"sample.wav", FileMode.Create, FileAccess.Write, FileShare.Write))
                            {
                                await dataStream.CopyToAsync(fileStream).ConfigureAwait(false);
                                fileStream.Close();
                            }
                            Console.WriteLine("\n音频文件已保存,按任意键结束程序.");
                            Console.ReadLine();
                        }
                    }
                }
            }
        }
    }
}
