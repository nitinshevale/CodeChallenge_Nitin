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
    public class CustomerController : Controller
    {
        static string customer_id;

        public IActionResult Customer(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                customer_id = id;
                return View(formatCustomer(CustomerListController.customerRepo.Customers.Find(item => item.id.ToString() == id)));
            }
            
            return View();
        }

        public IActionResult Push(string firstName, string lastName, string email,
            string phone_number, string country_code, string gender, string balance)
        {
            Customer customer = new Customer();
            if (customer_id == null)
                customer.id = CustomerListController.customerRepo.Customers.Last().id + 1;
            else
                customer.id = int.Parse(customer_id);

            customer.firstName = firstName;
            customer.lastName = lastName;
            customer.email = email;
            customer.phone_number = phone_number;
            customer.country_code = country_code;
            customer.gender = gender;
            customer.balance = balance;

            customer = formatCustomer(customer);
            var customerErrors = validationErrors(customer);

            if (!customerErrors.Any())
            {
                var customerSerialized = System.Text.Json.JsonSerializer.Serialize(customer);
                var customerValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(customerSerialized);

                try
                {
                    var client = new RestClient();
                    var request = new RestRequest("https://getinvoices.azurewebsites.net/api/Customer/" 
                        + ((customer_id != null) ? customer_id : ""), Method.Post);
                    customer_id = null;

                    request.RequestFormat = DataFormat.Json;
                    request.AddJsonBody(customerValues);

                    var response = client.ExecutePostAsync(request);

                    if (!response.Result.IsSuccessful)
                    {
                        Debug.WriteLine("Server Error : " + response);
                        return Json(new { result = "Error" });
                    }
                    Debug.WriteLine("Created Customer : " + customer.firstName);

                    return Json(new { result = "Success" });
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                    return Json(new { result = e });
                }
            }
            return Json(new { result = "ValidationFailed", validation_errors = customerErrors });
        }

        public List<string> validationErrors(Customer customer)
        {
            var errors = new List<string>();

            string regex_name = "^[^\\d]+$";
            string regex_email = "^[_a-z0-9-]+(.[a-z0-9-]+)@[a-z0-9-]+(.[a-z0-9-]+)*(.[a-z]{2,4})$";
            string regex_phone = "^([a-zA-Z,#/ \\.\\(\\)\\-\\+\\*]*[0-9]){7}[0-9a-zA-Z,#/ \\.\\(\\)\\-\\+\\*]*$";
            string regex_balance = "^[+-]?[0-9]{1,3}(?:,?[0-9]{3})*(?:" + new string("\\")[0] + ".[0-9]{2})?$";

            if (!String.IsNullOrEmpty(customer.firstName) && !Regex.IsMatch(customer.firstName, regex_name))
                errors.Add("Firstname");
            if (!String.IsNullOrEmpty(customer.lastName) && !Regex.IsMatch(customer.lastName, regex_name))
                errors.Add("Lastname");
            if (!String.IsNullOrEmpty(customer.phone_number) && !Regex.IsMatch(customer.phone_number, regex_phone))
                errors.Add("Phone");
            if (!String.IsNullOrEmpty(customer.balance) && !Regex.IsMatch(customer.balance, regex_balance))
                errors.Add("Balance");
            if (!String.IsNullOrEmpty(customer.email) && !Regex.IsMatch(customer.email, regex_email))
                errors.Add("Email");
            else if (String.IsNullOrEmpty(customer.email))
                errors.Add("Email_Empty");

            return errors;
        }

        public Customer formatCustomer(Customer customer)
        {
            customer.gender = customer.gender[0].ToString();

            if (String.IsNullOrEmpty(customer.balance) || customer.balance == "0.0")
                customer.balance = "0";

            if(!String.IsNullOrEmpty(customer.country_code))
            {
                customer.country_code = customer.country_code.Substring(0,2);
            }

            if (!String.IsNullOrEmpty(customer.phone_number))
            {
                customer.phone_number.Replace("&#x2B;", "+");
                if (customer.country_code.Contains("US"))
                {
                    if (!customer.phone_number.StartsWith("+"))
                        customer.phone_number = "+1 " + Regex.Replace(customer.phone_number, @"(\d{3})(\d{3})(\d{4})", "$1-$2-$3");
                }
                else if (customer.country_code.Contains("SE"))
                {
                    if (!customer.phone_number.StartsWith("+"))
                        customer.phone_number = "+46 " + Regex.Replace(customer.phone_number, @"(\d{2})(\d{3})(\d{2})(\d{2})", "$1 $2 $3 $4");
                }
            }

            //The server only seems to accept US country codes, otherwise it throws 500 Server Error
            customer.country_code = "US";

            return customer;
        }

    }
}
