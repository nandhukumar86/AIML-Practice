using System;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyApp.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http;

namespace MyApp.Controllers
{
    public class HomeController : Controller
    {
        Uri baseAddress = new Uri("http://127.0.0.1:5000/");
        HttpClient client;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            client = new HttpClient();
            client.BaseAddress = baseAddress;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost("FileUpload")]
        public async Task<IActionResult> Index(IFormFile file)
        {
            var filePath = Path.Combine(  
                     Directory.GetCurrentDirectory(),  
                     "UploadedFiles","DataSet.csv");
            

            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    ViewBag.UploadStatus = true;
                }
            }

            return View("Index");
        }

        [HttpPost("Regressor")]
        public async Task<IActionResult> Regressor(string target)
        {
            var filePath = Path.Combine(  
                     Directory.GetCurrentDirectory(),  
                     "UploadedFiles","DataSet.csv");

            bool flag = false;

            using(var reader = new StreamReader(filePath))
            {

                var line = reader.ReadLine();
                var columns = line.Split(',');

                foreach (var item in columns)
                {
                    if(item.Trim() == target)
                    {
                        flag = true;
                        ViewBag.RegressorMessage = $"Column Available";

                        if(target != "Signal_Strength")
                        {
                            ViewBag.RegressorMessage = $"Column Available, Apt Column will be Signal_Strength";
                        }

                        break;
                    }
                }                
            }
            ViewBag.UploadStatus = true;
            if(!flag) ViewBag.RegressorMessage = "Column not Available.";
            if(flag)
            {
                var fileContent = new ByteArrayContent(Bytes);  
                fileContent.Headers.Add("Content-Disposition", "attachment; filename=Signal.csv"); 

                var fileContent = new ByteArrayContent(Bytes);  
                fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = "Signal.csv"};  
                content.Add(fileContent); 
                
                var result = client.PostAsync(requestUri, content).Result;
                if(result.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    ViewBag.RegressorMessage = "yes";
                }
                else
                {
                    ViewBag.RegressorMessage = "no";
                }
            }
            return View("Index");
        }

        [HttpPost("Classifier")]
        public async Task<IActionResult> Classifier(string target)
        {
            var filePath = Path.Combine(  
                     Directory.GetCurrentDirectory(),  
                     "UploadedFiles","DataSet.csv");

            bool flag = false;

            using(var reader = new StreamReader(filePath))
            {

                var line = reader.ReadLine();
                var columns = line.Split(',');

                foreach (var item in columns)
                {
                    if(item.Trim() == target)
                    {
                        flag = true;
                        ViewBag.ClassifierMessage = $"Column Available";

                        if(target != "Signal_Strength")
                        {
                            ViewBag.ClassifierMessage = $"Column Available, Apt Column will be Signal_Strength";
                        }

                        break;
                    }
                }                
            }
            ViewBag.UploadStatus = true;
            if(!flag) ViewBag.ClassifierMessage = "Column not Available.";
            return View("Index");
        }
        
    }
}
