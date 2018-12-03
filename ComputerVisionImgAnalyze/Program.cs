using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace ComputerVisionImgAnalyze
{
    /// <summary>
    /// 图片场景分析
    /// </summary>
    class Program
    {
        // subscriptionKey = "0123456789abcdef0123456789ABCDEF"
        private const string apiKey = "";

        // localImagePath = @"C:\Documents\LocalImage.jpg"
        private const string localImagePath = @"imgs/1.jpg";

        private const string remoteImageUrl = "https://gss2.bdstatic.com/9fo3dSag_xI4khGkpoWK1HF6hhy/baike/w%3D268%3Bg%3D0/sign=80c78b32a4d3fd1f3609a53c08754222/6c224f4a20a446230bbbbd7e9422720e0cf3d7bc.jpg";

        // 指定要返回的特性
        private static readonly List<VisualFeatureTypes> features =
            new List<VisualFeatureTypes>()
        {
            VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
            VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Tags
        };

        static void Main(string[] args)
        {
            var computerVision = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(apiKey),
                new System.Net.Http.DelegatingHandler[] { });

            // 指定Azure区域
            computerVision.Endpoint = "https://westcentralus.api.cognitive.microsoft.com";

            Console.WriteLine("分析图像中 ...");

            // 远程图片
            AnalyzeRemoteAsync(computerVision, remoteImageUrl).GetAwaiter().GetResult();

            // 本地图片
            //AnalyzeLocalAsync(computerVision, localImagePath).GetAwaiter().GetResult();


            Console.WriteLine("按任意键键退出...");
            Console.ReadLine();
        }

        // 分析远程图像
        private static async Task AnalyzeRemoteAsync(
            ComputerVisionClient computerVision, string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                Console.WriteLine("\n无效的图片链接:\n{0} \n", imageUrl);
                return;
            }

            ImageAnalysis analysis = await computerVision.AnalyzeImageAsync(imageUrl, features);

            DisplayResults(analysis, imageUrl);
            DisplayImgTag(analysis);
        }

        // 分析本地图像
        private static async Task AnalyzeLocalAsync(
            ComputerVisionClient computerVision, string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                Console.WriteLine("\n无法打开或读取本地图像路径:\n{0} \n", imagePath);
                return;
            }

            using (Stream imageStream = File.OpenRead(imagePath))
            {
                ImageAnalysis analysis = await computerVision.AnalyzeImageInStreamAsync(
                    imageStream, features);

                DisplayResults(analysis, imagePath);
                DisplayImgTag(analysis);
            }
        }

        // 显示图像最相关的标题
        private static void DisplayResults(ImageAnalysis analysis, string imageUri)
        {
            Console.WriteLine("\r\n\r\n{0}", imageUri);
            foreach (var caption in analysis.Description.Captions)
            {
                Console.WriteLine("\r\n{0}\r\n", caption.Text);
            }
           
        }

        // 展示标签
        private static void DisplayImgTag(ImageAnalysis analysis)
        {
            foreach (var tag in analysis.Tags)
            {
                Console.WriteLine("标签名称:{0}       {1}%", tag.Name, Math.Round(tag.Confidence * 100, 2));
            }
        }
    }
}
