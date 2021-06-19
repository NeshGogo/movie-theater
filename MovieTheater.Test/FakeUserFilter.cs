using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Test
{
    class FakeUserFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, "userTest@test.com"),
                new Claim(ClaimTypes.Name, "userTest@test.com"),
                new Claim(ClaimTypes.NameIdentifier, "52736a28-633f-496c-9c2e-3d1fb986a9fd"),
            }));

            await next();
        }
    }
}
