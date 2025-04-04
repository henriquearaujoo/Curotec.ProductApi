using System.Linq.Expressions;

namespace Curotec.ProductApi.Domain.Specifications;

public abstract class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; protected set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
    public Expression<Func<T, object>>? OrderBy { get; protected set; }
    public Expression<Func<T, object>>? OrderByDescending { get; protected set; }
    public int? Skip { get; protected set; }
    public int? Take { get; protected set; }
    public bool IsPagingEnabled { get; protected set; }

    protected void AddInclude(Expression<Func<T, object>> include)
    {
        Includes.Add(include);
    }

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }
}