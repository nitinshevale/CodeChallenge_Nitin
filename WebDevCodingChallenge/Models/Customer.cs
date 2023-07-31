using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebDevCodingChallenge.Models
{
    public class Customer
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string country_code { get; set; }
        public string gender { get; set; }
        public string balance { get; set; }

        public bool isEmpty()
        {
            return (id == 0
                    && firstName == null 
                    && lastName == null 
                    && email == null 
                    && phone_number == null
                    && country_code == null
                    && gender == null
                    && balance == null);
        }

    }
}
