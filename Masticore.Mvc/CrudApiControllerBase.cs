using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Masticore.Mvc
{
    /// <summary>
    /// A base class for API Controller supporting Create (POST), Read (GET, GET by Id), Update (PUT), and Delete (DELETE) REST actions over an ICrudAsync object
    /// If you need to customize the base class at all - such as custom routes - better to just start from ApiController
    /// </summary>
    /// <typeparam name="ModelType">A class that implements the IIdentifiable interface </typeparam>
    /// <typeparam name="KeyType">A key, which should be the same as the ICrudAsync entitytype (usually string or int)</typeparam>
    public abstract class CrudApiControllerBase<ModelType, KeyType, ICrudType> : ApiController
        where ModelType : class, IIdentifiable<KeyType>
        where ICrudType : ICrud<ModelType, KeyType>
    {
        /// <summary>
        /// Service originally initialized with this object
        /// </summary>
        private ICrudType _service;

        /// <summary>
        /// Constructor for DI
        /// </summary>
        /// <param name="service"></param>
        protected CrudApiControllerBase(ICrudType service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all models in the ICrudAsync
        /// Default Route: GET: api/[ControllerName]
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ModelType>> Get()
        {
            return await _service.ReadAllAsync();
        }

        /// <summary>
        /// Gets a single model from the ICrudAsync based on the given id
        /// Default Route: GET: api/[ControllerName]/:id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ModelType> Get(KeyType id)
        {
            return await _service.ReadAsync(id);
        }

        /// <summary>
        /// Creates a new model in the system, based on the given template for the object
        /// This returns the newly created object in the response
        /// Default Route: POST: api/[ControllerName]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ModelType> Post([FromBody]ModelType model)
        {
            return await _service.CreateAsync(model);
        }

        /// <summary>
        /// Updates the given model, finding by id
        /// Default Route: PUT: api/[ControllerName]/:id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual async Task<ModelType> Put(KeyType id, [FromBody]ModelType model)
        {
            model.Id = id;
            return await _service.UpdateAsync(model);
        }

        /// <summary>
        /// Deletes the model with the given id
        /// Default Route: DELETE: api/[ControllerName]/:id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual async Task Delete(KeyType id)
        {
            await _service.DeleteAsync(id);
        }
    }
}
