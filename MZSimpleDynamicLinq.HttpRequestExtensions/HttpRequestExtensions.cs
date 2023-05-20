using Microsoft.AspNetCore.Http;
using MZSimpleDynamicLinq.Core;
using MZSimpleDynamicLinq.Core.HttpPostBaseClass;
using MZSimpleDynamicLinq.Core.HttpPostBaseFunction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        public static async Task<LinqDataRequest> ToLinqDataRequestHttpPostRequest(this HttpRequest request)
        {
            string requestBody = await new StreamReader(request.Body).ReadToEndAsync();

            var searchvalue = Function.GetSearchValue(requestBody);
            string text = searchvalue.Item1.ToString();// draw
            string text2 = searchvalue.Item2.ToString();//start
            string text3 = searchvalue.Item3.ToString();//lenght
            string text4 = searchvalue.Item4;//searchvalue

            int take = text3 != null ? Math.Min(Convert.ToInt32(text3), MaximumItemsPerPage) : MaximumItemsPerPage;
            int skip = text2 != null ? Math.Min(Convert.ToInt32(text2), MaximumViewablePage) : 0;


            var rootObject = JsonConvert.DeserializeObject<RootObject>(requestBody);

            int num = 0;
            List<Sort> list = new List<Sort>();
            var coloumnIDThatHaveSort = rootObject.Order?.Select(ss => ss.Column).ToArray();
            for (int i = 0; i < coloumnIDThatHaveSort.Count(); i++)
            {

                var coloumnname = rootObject.Columns[coloumnIDThatHaveSort[i]].Name;
                Sort item = new Sort
                {
                    Dir = rootObject.Order.FirstOrDefault(ss => ss.Column == coloumnIDThatHaveSort[i]).Dir,
                    Field = coloumnname
                };


                list.Add(item);
            }

            var filteresItemColoumns = rootObject.Columns.Select(ss => new
            {
                coloumName = (string?)ss.Name,
                filterValue = (string?)ss.Search.Value,
                searchableValue = (bool)ss.Searchable,
            }).Where(ss => !string.IsNullOrWhiteSpace(ss.filterValue)).ToArray();

            var CountOfFilteredColoumn = rootObject.Columns.Select(ss => ss.Search)
                .Where(ss => !string.IsNullOrWhiteSpace(ss.Value)).Count();

            Filter filter = new Filter();
            List<Filter> list2 = new List<Filter>();
            for (int j = 0; j < CountOfFilteredColoumn; j++)
            {
                string? text6 = filteresItemColoumns[j].coloumName;//coloumnName
                string? text7 = filteresItemColoumns[j].searchableValue.ToString();//searchable
                string? text8 = filteresItemColoumns[j].filterValue;//filterValue
                if (!string.IsNullOrEmpty(text6) && !string.IsNullOrEmpty(text8) && !(text7.ToLower() != "true"))
                {
                    list2.Add(new Filter
                    {
                        Logic = "and",
                        Field = text6,
                        Operator = "contains",
                        Value = text8.Replace("(", "").Replace(")", "")
                    });
                }
            }

            if (list2.Count > 0)
            {
                filter.Logic = "and";
                filter.Filters = list2;
            }

            return new LinqDataRequest
            {
                Skip = skip,
                Take = take,
                Filter = filter,
                Sort = list
            };
        }
    }
}
