using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Masticore.Mvc
{
    /// <summary>
    /// A self-container class that can render CSHTML views - EG, sending e-mails from ApiControllers
    /// </summary>
    public class ViewWriter
    {
        /// <summary>
        /// Empty controller class for parenting the independent views
        /// </summary>
        private class EmptyController : Controller { }

        Dictionary<string, IView> _viewCache = new Dictionary<string, IView>();
        ControllerContext _controllerContext;

        /// <summary>
        /// Renders a string for the given view, model, and optional layout path
        /// </summary>
        /// <param name="viewPath"></param>
        /// <param name="model"></param>
        /// <param name="layoutPath"></param>
        /// <returns></returns>
        public string RenderString(string viewPath, object model, string layoutPath = "")
        {
            using (var sw = new StringWriter())
            {
                ControllerContext controllerContext = GetControllerContext();

                IView view = GetView(viewPath, layoutPath, controllerContext);

                view.Render(new ViewContext(controllerContext, view, new ViewDataDictionary { Model = model }, new TempDataDictionary(), sw), sw);

                return sw.ToString();
            }
        }

        /// <summary>
        /// Returns the given View from the engine, potentially returning a cached version
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="layoutName"></param>
        /// <param name="controllerContext"></param>
        /// <returns></returns>
        private IView GetView(string viewName, string layoutName, ControllerContext controllerContext)
        {
            string viewId = string.Format("{0}{1}", viewName, layoutName);

            if (!_viewCache.ContainsKey(viewId))
                _viewCache[viewId] = ViewEngines.Engines.FindView(controllerContext, viewName, layoutName).View;

            return _viewCache[viewId];
        }

        /// <summary>
        /// Returns the controller context for this instance, potentially returning a cached object
        /// </summary>
        /// <returns></returns>
        private ControllerContext GetControllerContext()
        {
            if (_controllerContext == null)
            {
                var httpContext = new HttpContextWrapper(HttpContext.Current);

                // point to an empty controller
                var routeData = new RouteData();
                routeData.Values.Add("controller", "EmptyController");

                _controllerContext = new ControllerContext(new RequestContext(httpContext, routeData), new EmptyController());
            }

            return _controllerContext;
        }
    }
}
