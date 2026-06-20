using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using ClinicMicroServices.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ClinicMicroServices.Persistence.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        private readonly ClinicDbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(ClinicDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            // خليه يسيب EF يرمي لو entity null (هيطلع ArgumentNullException جوه EF)
            await _dbSet.AddAsync(entity);
        }

        public async Task<int> CountAsync(ISpecifications<TEntity, TKey> specifications)
        {
            // لو spec null ده غلط من service/controller (مش repo)
            return await SpecificationEvaluator.CreateQuery<TEntity, TKey>(_dbSet.AsQueryable(), specifications)
                                              .CountAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> specifications)
        {
            return await SpecificationEvaluator.CreateQuery<TEntity, TKey>(_dbSet.AsQueryable(), specifications)
                                              .AsNoTracking()
                                              .ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

    //    public async Task<IEnumerable<TEntity>> GetAllAsync(
    //params Expression<Func<TEntity, object>>[] includes)
    //    {
    //        IQueryable<TEntity> query = _dbSet;

    //        foreach (var include in includes)
    //            query = query.Include(include);

    //        return await query.AsNoTracking().ToListAsync();
    //    }

        public async Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, TKey> specifications)
        {
            return await SpecificationEvaluator.CreateQuery<TEntity, TKey>(_dbSet.AsQueryable(), specifications)
                                              .AsNoTracking()
                                              .FirstOrDefaultAsync();
        }
        public async Task<bool> AnyAsync(ISpecifications<TEntity, TKey> spec)
        {
            return await SpecificationEvaluator.CreateQuery<TEntity, TKey>(_dbSet.AsQueryable(), spec)
                                              .AsNoTracking()
                                              .AnyAsync();
        }

        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public async Task<IReadOnlyList<TEntity>> GetAllWithSpecAsync(ISpecifications<TEntity, TKey> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        private IQueryable<TEntity> ApplySpecification(ISpecifications<TEntity, TKey> spec)
        {
            return SpecificationEvaluator.CreateQuery(_dbContext.Set<TEntity>(), spec);
        }

    }
}
