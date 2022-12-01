using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Services
{
    public static class DateService
    {
        public static string ToDateCreatedViewModel(this DateTime created) 
            => $"{created.ToString("d MMMM yyyy H:mm")}";
    }
}
