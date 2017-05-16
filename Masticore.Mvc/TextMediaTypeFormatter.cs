using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Masticore.Mvc
{
    /// <summary>
    /// MediaTypeFormatter implemeneting the reading of plain text requests
    /// Register this formatter in the WebApiConfig.cs file in App_Start of an MVC project
    /// public static class WebApiConfig
    /// {
    ///    public static void Register(HttpConfiguration config)
    ///    {
    ///       config.Formatters.Add(new TextMediaTypeFormatter());
    /// </summary>
    public class TextMediaTypeFormatter : MediaTypeFormatter
    {
        /// <summary>
        /// Constructor
        /// Loads the supported types for this class
        /// </summary>
        public TextMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/xml"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));
        }

        /// <summary>
        /// Reads the content from the stream as a UTF8-encoded string
        /// </summary>
        /// <param name="type"></param>
        /// <param name="readStream"></param>
        /// <param name="content"></param>
        /// <param name="formatterLogger"></param>
        /// <returns></returns>
        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            try
            {
                var memoryStream = new MemoryStream();
                readStream.CopyTo(memoryStream);
                var s = Encoding.UTF8.GetString(memoryStream.ToArray());
                taskCompletionSource.SetResult(s);
            }
            catch (Exception e)
            {
                taskCompletionSource.SetException(e);
            }
            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Returns true if the type is string
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool CanReadType(Type type)
        {
            return type == typeof(string);
        }

        /// <summary>
        /// Returns false - this cannot write any types
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool CanWriteType(Type type)
        {
            return false;
        }
    }
}
