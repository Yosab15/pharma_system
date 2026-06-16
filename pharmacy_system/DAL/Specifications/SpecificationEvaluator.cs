using DAL.Specifications;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repo.Impelementation
{
    public static class SpecificationEvaluator<T>
        where T : class
    {
        public static IQueryable<T> GetQuery(
            IQueryable<T> inputQuery,
            ISpecification<T> spec)
        {
            var query = inputQuery;

            // WHERE
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            // INCLUDE
            query = spec.Includes.Aggregate(query,
                (currentQuery, include) =>
                    currentQuery.Include(include));

            // ORDER BY
            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }

            if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(
                    spec.OrderByDescending);
            }

            // PAGINATION
            if (spec.IsPagingEnabled)
            {
                query = query
                    .Skip(spec.Skip)
                    .Take(spec.Take);
            }

            return query;
        }
    }
}