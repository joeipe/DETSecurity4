using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DETSecurity4.Client.Models
{
    public class UserDetailsViewModel
    {
        public string Address { get; private set; } = string.Empty;
        public string Role { get; private set; } = string.Empty;
        public string Weatherforecast { get; private set; } = string.Empty;
        public UserDetailsViewModel(string address, string role, string weatherforecast)
        {
            Address = address;
            Role = role;
            Weatherforecast = weatherforecast;
        }
    }
}
