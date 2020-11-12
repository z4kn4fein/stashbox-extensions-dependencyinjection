using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Stashbox.AspNetCore.Multitenant
{
    internal class StashboxMultitenantStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<StashboxMultitenantMiddleware>();
                next(builder);
            };
        }
    }
}
