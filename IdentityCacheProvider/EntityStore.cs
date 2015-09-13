using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace InterSystems.AspNet.Identity.Cache
{
    /// <summary>
    ///     EntityFramework based IIdentityEntityStore that allows query/manipulation of a TEntity set
    /// </summary>
    /// <typeparam name="TEntity">Concrete entity type, i.e .User</typeparam>
    internal class EntityStore<TEntity> where TEntity : class
    {
        /// <summary>
        ///     Constructor that takes a Context
        /// </summary>
        /// <param name="context"></param>
        public EntityStore(DbContext context)
        {
            Context = context;
            DbEntitySet = context.Set<TEntity>();
        }

        /// <summary>
        ///     Context for the store
        /// </summary>
        public DbContext Context { get; private set; }

        /// <summary>
        ///     Used to query the entities
        /// </summary>
        public IQueryable<TEntity> EntitySet
        {
            get { return DbEntitySet; }
        }

        /// <summary>
        ///     EntitySet for this store
        /// </summary>
        public DbSet<TEntity> DbEntitySet { get; private set; }

        /// <summary>
        ///     FindAsync an entity by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task<TEntity> GetByIdAsync(object id)
        {
            return id != null ? DbEntitySet.FindAsync(id) : null;
        }

        /// <summary>
        ///     Insert an entity
        /// </summary>
        /// <param name="entity"></param>
        public void Create(TEntity entity)
        {
            if (entity != null)
            {
                DbEntitySet.Add(entity);
            }
        }

        /// <summary>
        ///     Mark an entity for deletion
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(TEntity entity)
        {
            if (entity != null)
            {
                DbEntitySet.Remove(entity);
            }
        }

        /// <summary>
        ///     Update an entity
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(TEntity entity)
        {
            if (entity != null)
            {
                Context.Entry(entity).State = EntityState.Modified;
            }
        }
    }
}