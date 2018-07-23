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
        const string subscriptionKey = "8f33f3675c7345e7a54a12a6a4d7c681";
        const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v2.0/ocr";
        public IActionResult Index(bool? saveChangesError=false, bool? noText=false, bool? noFilePath=false) //the optional parameters are used to provide error message to the user in the view in case of an exception in POST
        {
            if (saveChangesError.GetValueOrDefault()) //return current value or default for the nullable
            {
                ViewData["ErrorMessage"] =
                    "Make sure to select a license plate before proceeding."; //Generated license plate wasnt selected in the view (multiple license plates are passed if found.
            }
            if (noText.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "No license plate text generated. Please try again with a clearer picture."; //the OCR API did not manage to recover a complete and accurate license plate text
            }
            if (noFilePath.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "No file selected. Please select a file."; //Presses upload without selecting a file
            }
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
                try
                {
                    HttpContent content = new StreamContent(file.OpenReadStream());
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    var response = await client.PostAsync(uri, content);
                    result = await response.Content.ReadAsStringAsync();
                }
                catch (NullReferenceException) //only seems to happen when a file(path) is not provided
                {
                    return RedirectToAction("Index", "Upload", new { noFilePath = true }); //redirect to GET
                }
               
            }
            try
            {
                ViewBag.result = Support.ExtractLicense(result); //calls the function that extracts the license plate string from the JSON 
            }
            catch (ArgumentOutOfRangeException)
            {
                return RedirectToAction("Index", "Upload", new { noText = true }); //redirect to GET
            }
           return View();
        }
    }
}