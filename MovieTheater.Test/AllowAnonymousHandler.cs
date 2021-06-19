using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Test
{
    public class AllowAnonymousHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            context.PendingRequirements.ToList()
                .ForEach(requierements => context.Succeed(requierements));
            return Task.CompletedTask;
        }
    }
}
