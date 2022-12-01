using Shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Services
{
    public static class CustomerService
    {
        public static string ToCustomerFullName(this Customer customer)
            => $"{customer.Name} {customer.Surname}";
        
    }
}
