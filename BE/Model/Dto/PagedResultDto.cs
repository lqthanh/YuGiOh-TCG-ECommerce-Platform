using System;
using System.Collections.Generic;

namespace BE.Model.Dto
{
    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    }
}
