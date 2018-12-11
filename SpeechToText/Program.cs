using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.IO;
using System.Threading.Tasks;


namespace SpeechToText
{
    class Program
    {
        // 官方文档地址：https://docs.microsoft.com/zh-cn/azure/cognitive-services/speech-service/quickstart-csharp-dotnetcore-windows

        // subscription key
        static string apiKey = "";
        // 地区
        static string region = "eastasia";
        // 语言
        static string speechRecognitionLanguage = "zh-CN";

        // 音频流格式化配置
        static AudioStreamFormat AudioStreamFormat;

        static void Main()
        {
            //// 短语音实时识别
            ////RecognizeSortSpeechAsync().Wait();

            //// 长语音实时识别
            //RecognizeLongSpeechAsync().Wait();

            // 从音频文件识别
            var filePath = @"azure语音测试_16bit16khz.wav";
            //filePath = @"再别康桥.wav";
            //FormFile(filePath).Wait();

            // 从音频文件流识别
            /* 音频格式化配置
             * 此配置不可更改，官方目前只支持此标准
             * 官方文档链接：https://docs.microsoft.com/zh-cn/azure/cognitive-services/speech-service/how-to-use-audio-input-streams
             * */
            if (AudioStreamFormat == null)
            {
                byte channels = 1;// 1 通道
                byte bitsPerSample = 16;// 比特率 16
                uint samplesPerSecond = 16000;// 采样率 16000
                AudioStreamFormat = AudioStreamFormat.GetWaveFormatPCM(samplesPerSecond, bitsPerSample, channels);
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            FormStream(fileStream).Wait();


            Console.WriteLine("按任意键结束程序..");
            Console.ReadKey();
        }

        /// <summary>
        /// 短语音的识别(实时)
        /// </summary>
        /// <returns></returns>
        public static async Task RecognizeSortSpeechAsync()
        {
            var config = SpeechConfig.FromSubscription(apiKey, region);
            config.SpeechRecognitionLanguage = speechRecognitionLanguage; // 语言设置

            // 创建分析器
            using (var recognizer = new SpeechRecognizer(config))
            {

                Console.WriteLine("说点什么...");

                /*
                 * RecognizeOnceAsync() 在第一个语音被识别后返回，因此它仅适用于命令或查询等单一识别,
                 * 对于长时间运行的识别，使用 StartContinuousRecognitionAsync() 代替。
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

        /// <summary>
        /// 长语音的识别(实时)
        /// </summary>
        /// <returns></returns>
        public static async Task RecognizeLongSpeechAsync()
        {
            var config = SpeechConfig.FromSubscription(apiKey, region);
            config.SpeechRecognitionLanguage = speechRecognitionLanguage; // 语言设置

            var stopRecognition = new TaskCompletionSource<int>();

            // 创建分析器
            using (var recognizer = new SpeechRecognizer(config))
            {

                Console.WriteLine("说点什么...");

                // 订阅分析事件

                // 正在识别事件
                recognizer.Recognizing += (s, e) =>
                {
                    // 正在识别中
                };

                // 识别结果事件
                recognizer.Recognized += (s, e) =>
                {
                    if (e.Result.Reason == ResultReason.RecognizedSpeech)
                    {
                        Console.WriteLine(e.Result.Text);

                        if (e.Result.Text == "停止识别。")
                        {
                            // 停止识别
                            stopRecognition.TrySetResult(0);
                        }
                    }
                    else if (e.Result.Reason == ResultReason.NoMatch)
                    {
                        Console.WriteLine($"未匹配到: ");
                    }
                };

                // 识别取消事件
                recognizer.Canceled += (s, e) =>
                {
                    Console.WriteLine($"取消识别: 原因={e.Reason}");

                    if (e.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"取消识别: 错误码={e.ErrorCode}");
                        Console.WriteLine($"取消识别: 错误详情={e.ErrorDetails}");
                        Console.WriteLine($"取消识别: 请检查你的订阅信息");
                    }

                    stopRecognition.TrySetResult(0);
                };

                // 识别开始事件
                recognizer.SessionStarted += (s, e) =>
                {
                    Console.WriteLine("\n    识别开始.");
                };

                // 识别结束事件
                recognizer.SessionStopped += (s, e) =>
                {
                    Console.WriteLine("\n    识别结束.");
                };

                // 开始连续的识别。使用stopcontinuousrecognition()来停止识别。
                await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                // 等待完成。
                Task.WaitAny(new[] { stopRecognition.Task });
            }
        }


        /// <summary>
        /// 从文件读取音频 仅测试通过wav文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task FormFile(string filePath)
        {
            using (var audioConfig = AudioConfig.FromWavFileInput(filePath)) // 从文件读取
            {
                await SpeechRecognizer(audioConfig);
            }
        }


        /// <summary>
        /// 从音频流进行识别
        /// 
        /// 目前只支持特定的音频，详情查看：
        /// https://docs.microsoft.com/zh-cn/azure/cognitive-services/speech-service/how-to-use-audio-input-streams
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task FormStream(Stream stream)
        {
            using (var audioConfig = AudioConfig.FromStreamInput(new ReadPCMStream(stream), AudioStreamFormat)) // 从文件读取
            {
                await SpeechRecognizer(audioConfig);
            }
        }


        // 
        private static async Task SpeechRecognizer(AudioConfig audioConfig)
        {
            // 订阅信息配置
            var config = SpeechConfig.FromSubscription(apiKey, region);
            // 语言配置
            config.SpeechRecognitionLanguage = speechRecognitionLanguage;

            // 此处作为停止器
            var stopRecognition = new TaskCompletionSource<int>();

            using (var recognizer = new SpeechRecognizer(config, audioConfig))
            {
                // 订阅分析事件
                recognizer.Recognizing += (s, e) =>
                {
                    //Console.WriteLine(e.Result.Text);
                };

                recognizer.Recognized += (s, e) =>
                {
                    if (e.Result.Reason == ResultReason.RecognizedSpeech)
                    {
                        Console.WriteLine(e.Result.Text);
                    }
                    else if (e.Result.Reason == ResultReason.NoMatch)
                    {
                        Console.WriteLine($"未识别到数据: ");
                    }
                };

                recognizer.Canceled += (s, e) =>
                {
                    Console.WriteLine($"识别取消: 原因={e.Reason}");

                    if (e.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"识别取消: 错误码={e.ErrorCode}");
                        Console.WriteLine($"识别取消: 错误详情={e.ErrorDetails}");
                        Console.WriteLine($"识别取消: 请检查你的Azure订阅是否更新");
                    }

                    stopRecognition.TrySetResult(0);
                };

                recognizer.SessionStarted += (s, e) =>
                {
                    Console.WriteLine("\n    开始识别.");
                };

                recognizer.SessionStopped += (s, e) =>
                {
                    Console.WriteLine("\n    结束识别.");
                    stopRecognition.TrySetResult(0);
                };

                // 开始连续的识别。使用stopcontinuousrecognition()来停止识别。
                await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                // 等待完成。
                Task.WaitAny(new[] { stopRecognition.Task });

                // 停止识别
                await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            }
        }


    }

}