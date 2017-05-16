using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Masticore.Mvc
{
    /// <summary>
    /// Controller base class over an ICrudAsync
    /// </summary>
    public abstract class CrudController<ViewModelType, IdType> : Controller 
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
        public bool IsCloneEnabled { get; set; } = true;

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

        // GET: ControllerName
        public virtual async Task<ActionResult> Index()
        {
            if (!IsIndexEnabled)
                return HttpNotFound();

            return View(await Service.ReadAllAsync());
        }

        // GET: ControllerName/Details/5
        public virtual async Task<ActionResult> Details(IdType id)
        {
            if (!IsDetailsEnabled)
                return HttpNotFound();

            ViewModelType model = await Service.ReadAsync(id);
            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        // GET: ControllerName/Create
        public virtual async Task<ActionResult> Create()
        {
            if (!IsCreateEnabled)
                return HttpNotFound();

            await this.SetupCUView();
            return View();
        }

        // POST: ControllerName/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Create(ViewModelType viewModel)
        {
            if (!IsCreateEnabled)
                return HttpNotFound();

            if (ModelState.IsValid)
            {
                await Service.CreateAsync(viewModel);
                return RedirectToAction("Index");
            }

            await this.SetupCUView(viewModel);
            return View(viewModel);
        }

        // GET: ControllerName/Edit/5
        public virtual async Task<ActionResult> Edit(IdType id)
        {
            if (!IsEditEnabled)
                return HttpNotFound();

            ViewModelType model = await Service.ReadAsync(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            await this.SetupCUView(model);
            return View(model);
        }

        // POST: ControllerName/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

            await this.SetupCUView(viewModel);
            return View(viewModel);
        }

        // GET: ControllerName/Delete/5
        public virtual async Task<ActionResult> Delete(IdType id)
        {
            if (!IsDeleteEnabled)
                return HttpNotFound();

            ViewModelType model = await Service.ReadAsync(id);
            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        // POST: ControllerName/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> DeleteConfirmed(IdType id)
        {
            if (!IsDeleteEnabled)
                return HttpNotFound();

            await Service.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        // GET: ControllerName/Clone/5
        public virtual async Task<ActionResult> Clone(IdType id)
        {
            if (!IsCloneEnabled)
                return HttpNotFound();

            ViewModelType model = await Service.ReadAsync(id);
            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        // POST: ControllerName/Clone/5
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
