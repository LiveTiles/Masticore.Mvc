using System.Threading.Tasks;
using System.Web.Mvc;

namespace Masticore.Mvc
{
    /// <summary>
    /// Controller base class over an ICrudAsync
    /// </summary>
    public abstract class CrudControllerBase<ViewModelType, IdType> : Controller
        where ViewModelType : class, IIdentifiable<IdType>
    {
        #region Members

        /// <summary>
        /// Gets or sets a flag indicating if the Index action is enabled
        /// </summary>
        public bool IsIndexEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a flag indicating if the Details action is enabled
        /// </summary>
        public bool IsDetailsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a flag indicating if the Create action is enabled
        /// </summary>
        public bool IsCreateEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a flag indicating if the Edit action is enabled
        /// </summary>
        public bool IsEditEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a flag indicating if the Delete action is enabled
        /// </summary>
        public bool IsDeleteEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a flag indicating if the Clone action is enabled
        /// </summary>
        public bool IsCloneEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the repository for this CrudController
        /// </summary>
        protected virtual ICrud<ViewModelType, IdType> Service { get; set; }

        /// <summary>
        /// Performs setup of the Create and Update views
        /// By default, this does nothing
        /// </summary>
        protected virtual Task SetupCUView(ViewModelType viewModel = null)
        {
            return Task.FromResult(0);
        }

        #endregion

        #region CRUD Actions 

        /// <summary>
        /// Index action that shows a view of all models in the Service
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ActionResult> Index()
        {
            if (!IsIndexEnabled)
                return HttpNotFound();

            return View(await Service.ReadAllAsync());
        }

        /// <summary>
        /// Detail action that shows a single model via ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ActionResult> Details(IdType id)
        {
            if (!IsDetailsEnabled)
                return HttpNotFound();

            ViewModelType model = await Service.ReadAsync(id);
            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        /// <summary>
        /// Create action that GETs the create form for a single instance of the object
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ActionResult> Create()
        {
            if (!IsCreateEnabled)
                return HttpNotFound();

            await SetupCUView();
            return View();
        }

        /// <summary>
        /// Create action that POSTs the model and creates a new instance using the service
        /// Returns the user to the details of the newly created model 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Create(ViewModelType viewModel)
        {
            if (!IsCreateEnabled)
                return HttpNotFound();

            if (ModelState.IsValid)
            {
                var newModel = await Service.CreateAsync(viewModel);
                return RedirectToAction("Details", new { id = newModel.Id });
            }

            await SetupCUView(viewModel);
            return View(viewModel);
        }

        /// <summary>
        /// GETs the edit for for the model with the given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ActionResult> Edit(IdType id)
        {
            if (!IsEditEnabled)
                return HttpNotFound();

            ViewModelType model = await Service.ReadAsync(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            await SetupCUView(model);
            return View(model);
        }

        /// <summary>
        /// POSTs the model for editing, applying it to the service and returning to the details of the model
        /// Returns the user to the details for the model
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Edit(ViewModelType viewModel)
        {
            if (!IsEditEnabled)
                return HttpNotFound();

            if (ModelState.IsValid)
            {
                await Service.UpdateAsync(viewModel);
                return RedirectToAction("Details", new { id = viewModel.Id });
            }

            await SetupCUView(viewModel);
            return View(viewModel);
        }

        /// <summary>
        /// GETs the form for deleting the model with the given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ActionResult> Delete(IdType id)
        {
            if (!IsDeleteEnabled)
                return HttpNotFound();

            ViewModelType model = await Service.ReadAsync(id);
            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        /// <summary>
        /// POSTs the form for deleting the model with the given ID
        /// Returns the user to the index action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> DeleteConfirmed(IdType id)
        {
            if (!IsDeleteEnabled)
                return HttpNotFound();

            await Service.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// GETs the form for closing the given model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ActionResult> Clone(IdType id)
        {
            if (!IsCloneEnabled)
                return HttpNotFound();

            ViewModelType model = await Service.ReadAsync(id);
            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        /// <summary>
        /// POSTs the form confirming the clone for the model with the given ID
        /// Returns the user to the details for the newly created model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Clone")]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> CloneConfirmed(IdType id)
        {
            if (!IsCloneEnabled)
                return HttpNotFound();

            ViewModelType model = await Service.ReadAsync(id);

            if (model == null)
                return HttpNotFound();

            ViewModelType newModel = await Service.CreateAsync(model);

            return RedirectToAction("Details", new { id = newModel.Id });
        }

        #endregion
    }
}
