using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebDevCodingChallenge.Models
{
    public class CustomerListModel
    {
        public List<Customer> Customers { get; set; }
        public string ErrorMessage { get; set; }

        public CustomerListModel()
        {
            Customers = new List<Customer>();
        }

    }
}
