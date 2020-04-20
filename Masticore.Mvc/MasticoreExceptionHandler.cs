using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace Masticore.Mvc
{   /// <summary>
    /// The WebAPI errors that will not be caught traditionally, (e.g., 404 and 500 caused by routing)
    /// </summary>
    public class UncaughtErrorMessage : DelegatingHandler
    {

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {


            // Call the inner handler.
            var response = await base.SendAsync(request, cancellationToken);

            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.Unauthorized:

                    response.StatusCode = HttpStatusCode.NotFound;
#if !DEBUG
                     response.Content = new StringContent("There was a problem and this resource could not be found.");
#endif
                    break;
                case HttpStatusCode.InternalServerError:
                    response.StatusCode = HttpStatusCode.NotFound;
#if !DEBUG
                        response.Content = new StringContent("There was a problem.");
#endif
                    break;
                default:
                    break;

            }

            return response;
        }
    }

    /// <summary>
    /// The generic exception handler on the web api level
    /// </summary>
    public class MasticoreExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            System.Diagnostics.Trace.TraceError(context.Exception.Message);
            System.Diagnostics.Trace.Flush();

            context.Result = new TextPlainErrorResult
            {
                Request = context.ExceptionContext.Request,
                Content = context.Exception.Message
            };
#if !DEBUG
            //TODO: Make KVP more robust, to return or log different levels of data (e.g., simple consumable string, validation, admin contact etc.)
            if (context.Exception.Data.Contains("IsConsumable") && context.Exception.Data["IsConsumable"].ToString() == "true")
            {
                context.Result = new TextPlainErrorResult
                {
                    Request = context.ExceptionContext.Request,
                    Content = context.Exception.Message,
                };
            }
            else
            {
                context.Result = new TextPlainErrorResult
                {
                    Request = context.ExceptionContext.Request,
                    Content = "Oops! Sorry! Something went wrong.",
                };
            }

         
#endif



        }


        private class TextPlainErrorResult : IHttpActionResult
        {
            public HttpRequestMessage Request { get; set; }

            public string Content { get; set; }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(Content);
                response.RequestMessage = Request;
                return Task.FromResult(response);
            }
        }
    }
}
