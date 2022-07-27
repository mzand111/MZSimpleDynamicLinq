
using MZSimpleDynamicLinq.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace MZSimpleDynamicLinq.Extensions
{
	public static class QueryableExtensions
	{

		/// <summary>
		/// Applies paging, sorting and filtering over IQueryable using Dynamic Linq.
		/// </summary>
		/// <typeparam name="T">The type of the IQueryable.</typeparam>
		/// <param name="queryable">The IQueryable which paging, sorting and filtering would be applied to.</param>
		/// <param name="take">Page size.</param>
		/// <param name="skip">Pages to skip.</param>
		/// <param name="sort">Requested sort order.</param>
		/// <param name="filter">Requested filters.</param>

		/// <returns>A LinqDataResult object populated from the processed IQueryable.</returns>
		public static LinqDataResult<T> ToLinqDataResult<T>(this IQueryable<T> queryable, int take, int skip, IEnumerable<Sort> sort, Filter filter)
		{
			// Filter the data first
			queryable = Filter(queryable, filter);

			// Calculate the total number of records (needed for paging)
			var total = queryable.Count();	

			// Sort the data
			queryable = Sort(queryable, sort);

			// Finally page the data
			if (take > 0)
			{
				queryable = Page(queryable, take, skip);
			}

			return new LinqDataResult<T>
			{
				Data = queryable.ToList(),
				RecordsTotal = total,

			};
		}

		public static LinqDataResult<P> ToLinqDataResult<T,P>(this IQueryable<T> queryable, int take, int skip, IEnumerable<Sort> sort, Filter filter)
			where T:P
			
		{
			// Filter the data first
			queryable = Filter(queryable, filter);

			// Calculate the total number of records (needed for paging)
			var total = queryable.Count();

			// Sort the data
			queryable = Sort(queryable, sort);

			// Finally page the data
			if (take > 0)
			{
				queryable = Page(queryable, take, skip);
			}

			return new LinqDataResult<P>
			{
				Data = queryable.ToList().Cast<P>(),
				RecordsTotal = total,

			};
		}


		private static IQueryable<T> Filter<T>(IQueryable<T> queryable, Filter filter)
		{
			if (filter != null && filter.Logic != null)
			{
				// Collect a flat list of all filters
				var filters = filter.GetFlat();

				// Get all filter values as array (needed by the Where method of Dynamic Linq)
				var values = filters.Select(f => f.Value).ToArray();

				// generate expression e.g. Field1 = @0 And Field2 > @1
				string predicate = filter.ToExpression(filters);

				// Use the Where method of Dynamic Linq to filter the data
				queryable = queryable.Where(predicate, values);
			}

			return queryable;
		}

		

		private static IQueryable<T> Sort<T>(IQueryable<T> queryable, IEnumerable<Sort> sort)
		{
			if (sort != null && sort.Any())
			{
				// order by expression: e.g. Field1 asc, Field2 desc
				var ordering = String.Join(",", sort.Select(s => s.ToExpression()));

				// Use the OrderBy method of Dynamic Linq to sort the data
				return queryable.OrderBy(ordering);
			}

			return queryable;
		}

		private static IQueryable<T> Page<T>(IQueryable<T> queryable, int take, int skip)
		{
			return queryable.Skip(skip).Take(take);
		}
	}
}
