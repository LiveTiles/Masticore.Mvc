using System;
using System.Web.Mvc;

namespace Masticore.Mvc
{
    /// <summary>
    /// Filter based on RequireHttpsAttribute that will thoughtfully ignore HTTPS requirements if the request is local
    /// </summary>
    public class RequireSecureConnection : RequireHttpsAttribute
    {
        /// <summary>
        /// Called when the user is being authorized in the system
        /// This will skip authorization if the request is local
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext));
            }

            if (filterContext.HttpContext.Request.IsLocal)
            {
                // when connection to the application is local, don't do any HTTPS stuff
                return;
            }

            base.OnAuthorization(filterContext);
        }
    }
}
