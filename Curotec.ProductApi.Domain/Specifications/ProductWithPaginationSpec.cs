using Curotec.ProductApi.Domain.Entities;

namespace Curotec.ProductApi.Domain.Specifications;

public class ProductWithPaginationSpec : BaseSpecification<Product>
{
    public ProductWithPaginationSpec(string? search, int pageIndex, int pageSize, string? sort)
    {
        if (!string.IsNullOrEmpty(search))
            Criteria = p => p.Name.Contains(search);

        ApplyPaging((pageIndex - 1) * pageSize, pageSize);

        if (!string.IsNullOrEmpty(sort))
        {
            OrderBy = sort switch
            {
                "priceAsc" => p => p.Price,
                "priceDesc" => p => p.Price,
                _ => p => p.Name
            };
        }
        else
        {
            OrderBy = p => p.Name;
        }
    }
}