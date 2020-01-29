using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DETSecurity4.API.Authorization
{
    public class MustBeWeatherManHandler : AuthorizationHandler<MustBeWeatherManRequirement>
    {
        public MustBeWeatherManHandler()
        {

        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MustBeWeatherManRequirement requirement)
        {
            var endpoint = context.Resource as Endpoint;
            if (endpoint == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            /*
            //RouteData can be controller, action or id
            var imageId = filterContext.RouteData.Values["id"].ToString();

            if (!Guid.TryParse(imageId, out Guid imageIdAsGuid))
            {
                context.Fail();
                return Task.CompletedTask;
            }*/

            /*
            //Repository check can go here
            var ownerId = context.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;

            if (!_someRepository.IsImageOwner(imageIdAsGuid, ownerId))
            {
                context.Fail();
                return Task.CompletedTask;
            }*/

            // all checks out
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
