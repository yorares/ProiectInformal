using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stark.Models;

namespace Stark.Controllers
{
    public class UploadController : Controller
    {
        const string subscriptionKey = "76a6e557b9344a26a7ccd3af91d9a02e";
        const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v2.0/ocr";
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // full path to file in temp location
            var imageFilePath = Path.GetTempFileName();

            string requestParameters = "language=unk&detectOrientation=true";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            string result = null;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                HttpContent content = new StreamContent(file.OpenReadStream());
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                var response = await client.PostAsync(uri, content);
                result = await response.Content.ReadAsStringAsync();
            }
            ViewBag.result = Support.ExtractLicense(result);
            return View();
        }
    }
}