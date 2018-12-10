using Microsoft.CognitiveServices.Speech;
using System;
using System.Threading.Tasks;

namespace SpeechToText
{
    class Program
    {
        // subscription key
        static string apiKey = "";
        // 地区
        static string region = "";

        static void Main()
        {
            RecognizeSpeechAsync().Wait();
            Console.WriteLine("按任意键结束程序..");
            Console.ReadKey();
            Console.ReadKey();
        }


        public static async Task RecognizeSpeechAsync()
        {
            var config = SpeechConfig.FromSubscription(apiKey, region);

            // 创建分析器
            using (var recognizer = new SpeechRecognizer(config))
            {
                Console.WriteLine("说点什么...");

                /*
                 * RecognizeOnceAsync() 在第一个语音被识别后返回，因此它仅适用于命令或查询等单一识别,
                 * 对于长时间运行的识别，使用StartContinuousRecognitionAsync()代替。
                 */
                var result = await recognizer.RecognizeOnceAsync();

                // 检查结果
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"识别结果: {result.Text}");
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"识别失败: 语言无法被识别.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"识别已取消: 原因={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"识别已取消: 错误码={cancellation.ErrorCode}");
                        Console.WriteLine($"识别已取消: 错误详情={cancellation.ErrorDetails}");
                        Console.WriteLine($"识别已取消: 请检查订阅是否正常");
                    }
                }
            }
        }
    }
}