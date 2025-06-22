using System.Linq.Expressions;
using Core.Interfaces;

namespace Core.Specifications;

public class BaseSpecefication<T> : ISpecification<T>
{
    private readonly Expression<Func<T, bool>>? _criteria;

    protected BaseSpecefication(): this(null) {}

    public BaseSpecefication(Expression<Func<T, bool>>? criteria)
    {
        _criteria = criteria;
    }

    public bool IsDistinct { get; private set;}

    public bool IsPagingEnabled { get; private set;}

    public int Take { get; private set;}

    public int Skip { get; private set;}

    public Expression<Func<T, bool>>? Criteria => _criteria;

    public Expression<Func<T, object>>? OrderBy { get; private set;}

    public Expression<Func<T, object>>? OrderByDescending { get; private set;}

    public List<Expression<Func<T, object>>> Includes { get; } = [];

    public List<string> IncludeStrings { get; } = [];

    public IQueryable<T> ApplyCriteria(IQueryable<T> query)
    {
        if (Criteria is not null)
        {
            query = query.Where(Criteria);
        }

        return query;
    }

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
        OrderByDescending = orderByDescExpression;
    }

    protected void ApplyDistinct()
    {
        IsDistinct = true;
    }

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }
}

public class BaseSpecefication<T, TResult> : BaseSpecefication<T>, ISpecification<T, TResult>
{
    protected BaseSpecefication() : this(null) {}

    public BaseSpecefication(Expression<Func<T, bool>>? criteria) : base(criteria) {}

    public Expression<Func<T, TResult>>? Select { get; private set; }

    protected void AddSelect(Expression<Func<T, TResult>> selectExpression)
    {
        Select = selectExpression;
    }
}
