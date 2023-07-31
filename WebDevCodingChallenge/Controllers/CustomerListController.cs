using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http.Description;
using WebDevCodingChallenge.Models;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
using System.Text.RegularExpressions;

namespace WebDevCodingChallenge.Controllers
{
    public class CustomerListController : Controller
    {
        public static CustomerListModel customerRepo;

        public IActionResult CustomerList()
        {
            customerRepo = new CustomerListModel();
            populateCustomerRepo("https://getinvoices.azurewebsites.net/api/Customers");
            return View(customerRepo);
        }

        public HttpResponseMessage ResetTable(HttpRequestMessage request)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://getinvoices.azurewebsites.net/api/CreateCustomerList");
                HttpResponseMessage response = client.SendAsync(requestMessage).GetAwaiter().GetResult();
                Debug.WriteLine("TABLE RESET");
                return request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }

        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Delete, "https://getinvoices.azurewebsites.net/api/Customer/" + id);
                HttpResponseMessage response = client.SendAsync(requestMessage).GetAwaiter().GetResult();
                Debug.WriteLine("DELETED CUSTOMER : " + id);

                return request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }

        public void populateCustomerRepo(string url)
        {
            try
            {
                var customerList = new List<Customer>();

                var webClient = new WebClient();
                var jsonDownload = webClient.DownloadString(url);

                var result = JsonConvert.DeserializeObject<List<Customer>>(jsonDownload);

                foreach (var customer in result)
                {
                    if (!String.IsNullOrEmpty(customer.phone_number) && customer.phone_number.Contains("+46"))
                        customer.country_code = "SE";

                    if(!customer.isEmpty())
                        customerList.Add(customer);
                }

                customerRepo.Customers = customerList.OrderBy(x => x.id).ToList();
            }
            catch (Exception e)
            {
                customerRepo.ErrorMessage = "Error : " + e.ToString();
            }
        }

    }
}
