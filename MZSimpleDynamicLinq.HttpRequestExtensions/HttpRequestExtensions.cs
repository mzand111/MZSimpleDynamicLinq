using Microsoft.AspNetCore.Http;
using MZSimpleDynamicLinq.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MZSimpleDynamicLinq.HttpRequestExtensions
{
    public static class HttpRequestDataSourceExtension
    {

        public static int MaximumItemsPerPage = 500;
        public static int MaximumViewablePage = 5000;
        public static LinqDataRequest ToLinqDataRequest(this HttpRequest request)
        {

            var draw = request.Query["draw"].FirstOrDefault();
            var start = request.Query["start"].FirstOrDefault();
            var length = request.Query["length"].FirstOrDefault();

            var searchValue = request.Query["search[value]"].FirstOrDefault();
            int pageSize = length != null ? Math.Min( Convert.ToInt32(length), MaximumItemsPerPage) : MaximumItemsPerPage;
            int skip = start != null ? Math.Min(Convert.ToInt32(start), MaximumViewablePage) : 0;
            int recordsTotal = 0;

            List<Sort> sorts = new List<Sort>();
            //For now just support 20 filters
            for (int i = 0; i < 20; i++)
            {
                var sortColumn = request.Query["columns[" + request.Query["order[" + i + "][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = request.Query["order[" + i + "][dir]"].FirstOrDefault();
                if (string.IsNullOrEmpty(sortColumnDirection))
                {
                    break;
                }
                Sort sort = new Sort()
                {
                    Dir = sortColumnDirection,
                    Field = sortColumn
                };
                sorts.Add(sort);
            }

            Filter filter = new Filter();

            var fls = new List<Filter>();
            for (int i = 0; i < 20; i++)
            {
                var filterColumnName = request.Query["columns[" + i + "][name]"].FirstOrDefault();
                var columnSearchable = request.Query["columns[" + i + "][searchable]"].FirstOrDefault();
                var filterColumnValue = request.Query["columns[" + i + "][search][value]"].FirstOrDefault();

                if (string.IsNullOrEmpty(filterColumnName) || string.IsNullOrEmpty(filterColumnValue) || columnSearchable != "true")
                {
                    continue;
                }
                fls.Add(new Filter()
                {
                    Logic = "and",
                    Field = filterColumnName,
                    Operator = "contains",
                    Value = filterColumnValue.Replace("(", "").Replace(")", "")
                });
            }
            if (fls.Count > 0)
            {
                filter.Logic = "and";
                filter.Filters = fls;
            }


            return new LinqDataRequest()
            {
                Skip = skip,
                Take = pageSize,
                Filter = filter,
                Sort = sorts
            };
        }
    }
}
