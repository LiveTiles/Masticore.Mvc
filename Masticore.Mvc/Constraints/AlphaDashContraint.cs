using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;

namespace Masticore.Mvc.Constraints
{
    /// <summary>
    /// A constraint that only allows alphanumeric and dash characters
    /// This is used for page routes
    /// </summary>
    public class AlphaDashConstraint : IRouteConstraint
    {
        /// <summary>
        /// Regular Expression defining lowercase and uppercase A-Z, plus dash
        /// </summary>
        public static readonly Regex AlphaDashRegex = new Regex(@"[^a-zA-Z0-9\-]");

        /// <summary>
        /// Converts the given string to AlphaDash compliance
        /// Replaces spaces with dashes, then eliminates non-compliant characters
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ConvertToAlphaDash(string input)
        {
            input = input.Trim().ToLower().Replace(' ', '-');
            input = AlphaDashConstraint.AlphaDashRegex.Replace(input, "");
            return input;
        }

        /// <summary>
        /// Returns true if the given parameter in the RouteValueDictionary matches the AlphaDashRegex
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected bool IsParameterAlphaDash(string parameterName, RouteValueDictionary values)
        {
            var parameter = values[parameterName].ToString();
            return AlphaDashRegex.IsMatch(parameter);
        }

        /// <summary>
        /// Returns true if the parameter in the values dictionary matches the AlphaDashRegex (A-Z, a-z, plus dash)
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="route"></param>
        /// <param name="parameterName"></param>
        /// <param name="values"></param>
        /// <param name="routeDirection"></param>
        /// <returns></returns>
        public virtual bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return IsParameterAlphaDash(parameterName, values);
        }

    }
}