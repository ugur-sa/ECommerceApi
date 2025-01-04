using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Dtos.Pagination
{
    public class QueryParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "asc";
        public string? FilterBy { get; set; }
        public string? FilterValue { get; set; }
        public string? SearchTerm { get; set; }
    }
}
