using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IRestResponse NewGetProduct(string id)
        {
            string query = "https://localhost:44303/api/products/" + id;
            var client = new RestClient(query);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            return response;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IRestResponse NewAddProduct(string path)
        {
            var client = new RestClient("https://localhost:44303/api/products");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", path, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
        }

        public IActionResult ProductView(string id)
        {
            IRestResponse response = NewGetProduct(id);
            HttpStatusCode e = response.StatusCode;
            string w = e.ToString();
            if (response.IsSuccessful)
            {
                string resp = response.Content;
                resp = resp.Replace("id", "Id");
                resp = resp.Replace("code", "Code");
                resp = resp.Replace("value", "Value");
                if (id == null)
                {
                    List<Product> list = JsonSerializer.Deserialize<List<Product>>(resp);
                    return View(list);
                }
                else if (id.StartsWith("value"))
                {
                    List<Product> list = JsonSerializer.Deserialize<List<Product>>(resp);
                    return View(list);
                }
                else
                {
                    Product product = JsonSerializer.Deserialize<Product>(resp);
                    return View("OneProductView", product);
                }
            }
            else
            {
                return View("Page404");
            }
        }

        public IActionResult NewProduct(string path)
        {
            IRestResponse response = NewAddProduct(path);
            if (response.IsSuccessful)
            {
                ViewBag.resp = "Добавлено";
                return View();
            }
            else
            {
                string error = response.StatusCode.ToString();
                ViewBag.resp = "Ошибка: " + error;
                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
