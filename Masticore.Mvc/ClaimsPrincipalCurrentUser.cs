using System.Configuration;
using System.Security.Claims;
using System.Web;

namespace Masticore.Mvc
{
    /// <summary>
    /// Implementation of ICurrentUser provided via Active Directory
    /// </summary>
    public class ClaimsPrincipalCurrentUser : ICurrentUser
    {
        /// <summary>
        /// The URI for the objectidentifier claim type
        /// </summary>
        public const string ObjectIdentifierClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        public const string EmailClaimType = "emails";

        #region ICurrentUser

        /// <summary>
        /// Gets true if the current user is authenticated; otherwise, returns false
        /// </summary>
        public virtual bool IsAuthenticated
        {
            get
            {
                return HttpContext.Current.Request.IsAuthenticated;
            }
        }

        /// <summary>
        /// Gets the e-mail for the current user - returns the current Identity Name as a best-guess if the claim is not found
        /// </summary>
        public virtual string Email
        {
            get
            {
                var claim = ClaimsPrincipal.Current.FindFirst(EmailClaimType);
                return claim?.Value ?? ClaimsPrincipal.Current.Identity.Name;
            }
        }

        /// <summary>
        /// Gets the unique identifier (OID) for the current user
        /// </summary>
        public virtual string ExternalId
        {
            get
            {
                var claim = ClaimsPrincipal.Current.FindFirst(ObjectIdentifierClaimType);
                if (claim == null)
                    throw new System.Exception("Current User Does Not Have an Object Identifier Claim");
                return claim.Value;
            }
        }

        /// <summary>
        /// Gets the first name (given name) for the current user
        /// </summary>
        public virtual string FirstName
        {
            get
            {
                var claim = ClaimsPrincipal.Current.FindFirst(ClaimTypes.GivenName);
                return claim?.Value ?? "";
            }
        }

        /// <summary>
        /// Gets the last name (surname) for the current user
        /// </summary>
        public virtual string LastName
        {
            get
            {
                var claim = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Surname);
                return claim?.Value ?? "";
            }
        }

        #endregion
    }
}