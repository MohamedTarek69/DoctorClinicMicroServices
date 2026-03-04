using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using ClinicMicroServices.Persistence.Data.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ClinicDbContext _dbContext;

        private readonly Dictionary<Type, object> _repositories = [];

        public UnitOfWork(ClinicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : BaseEntity<TKey>
        {
            var entityType = typeof(TEntity);

            if (_repositories.TryGetValue(entityType, out object? repository))
                return (IGenericRepository<TEntity, TKey>)repository;

            var newRepository = new GenericRepository<TEntity, TKey>(_dbContext);
            _repositories[entityType] = newRepository;

            return newRepository;
        }

        public Task<int> SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}
