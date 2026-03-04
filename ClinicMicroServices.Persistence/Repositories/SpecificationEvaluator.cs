using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Persistence.Repositories
{
    internal static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> CreateQuery<TEntity, TKey>(
            IQueryable<TEntity> entryPoint,
            ISpecifications<TEntity, TKey> specifications)
            where TEntity : BaseEntity<TKey>
        {
            var query = entryPoint;

            if (specifications is null)
                return query;

            if (specifications.Critria is not null)
                query = query.Where(specifications.Critria);

            if (specifications.IncludeExpressions is not null && specifications.IncludeExpressions.Any())
                query = specifications.IncludeExpressions.Aggregate(query,
                    (current, include) => current.Include(include));

            if (specifications.OrderBy is not null)
                query = query.OrderBy(specifications.OrderBy);
            else if (specifications.OrderByDescending is not null)
                query = query.OrderByDescending(specifications.OrderByDescending);

            if (specifications.IsPaginated && specifications.Take > 0)
                query = query.Skip(specifications.Skip).Take(specifications.Take);


            return query;
        }
    }
}
