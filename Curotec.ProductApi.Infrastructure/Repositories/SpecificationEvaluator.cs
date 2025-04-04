using Curotec.ProductApi.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Curotec.ProductApi.Infrastructure.Repositories;

public static class SpecificationEvaluator<T> where T : class
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
    {
        var query = inputQuery;

        if (spec.Criteria is not null)
            query = query.Where(spec.Criteria);

        if (spec.OrderBy is not null)
            query = query.OrderBy(spec.OrderBy);
        else if (spec.OrderByDescending is not null)
            query = query.OrderByDescending(spec.OrderByDescending);

        if (spec.IsPagingEnabled && spec.Skip.HasValue && spec.Take.HasValue)
            query = query.Skip(spec.Skip.Value).Take(spec.Take.Value);

        foreach (var include in spec.Includes)
            query = query.Include(include);

        return query;
    }
}