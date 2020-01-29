using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DETSecurity4.API.Authorization
{
    public class MustBeWeatherManRequirement : IAuthorizationRequirement
    {
        public MustBeWeatherManRequirement()
        {

        }
    }
}
