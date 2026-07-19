using System.Linq;
using System.Threading.Tasks;
using BE.Model.Dto;
using Microsoft.EntityFrameworkCore;

namespace BE._extensions
{
    public static class QueryableExtensions
    {
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 100;

        public static (int Page, int PageSize) NormalizePagination(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = DefaultPageSize;
            if (pageSize > MaxPageSize) pageSize = MaxPageSize;
            return (page, pageSize);
        }

        public static async Task<PagedResultDto<T>> ToPagedResultAsync<T>(this IQueryable<T> query, int page, int pageSize)
        {
            (page, pageSize) = NormalizePagination(page, pageSize);
            var totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResultDto<T>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
            };
        }
    }
}
