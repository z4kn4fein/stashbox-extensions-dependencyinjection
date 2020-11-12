using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Stashbox.AspNetCore.Multitenant
{
    /// <summary>
    /// Represents a utility used to extract a tenant id from the current context.
    /// </summary>
    public interface ITenantIdExtractor
    {
        /// <summary>
        /// Extracts a tenant id from the current context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The extracted tenant id or null when no id found.</returns>
        Task<object> GetTenantIdAsync(HttpContext context);
    }
}
