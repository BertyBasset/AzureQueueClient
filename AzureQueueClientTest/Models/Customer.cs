using System;
using System.Collections.Generic;
using System.Text;

namespace ShedLab.Models {

    public class Address {
        public string HouseName { get; set; }
        public string HouseNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
    }

    public class ContactDetails {
        public string Phoine { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }


    public class Customer {
        public long ID { get; set; }
        public string Forenames { get; set; }
        public string Surname { get; set; }

        public Address Address { get; set; }
        public ContactDetails ContactDetails { get; set; }
    }
}

