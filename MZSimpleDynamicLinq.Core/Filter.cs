﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MZSimpleDynamicLinq.Core
{
    public class Filter
    {
        /// <summary>
        /// Name of the field for filtering. Value is expected to be <c>null</c> if the <c>Filters</c> property is set (So it is a combined filter).
        /// </summary>
        [DataMember(Name = "field")]
        public string? Field { get; set; }

        /// <summary>
        /// Gets or sets the filtering value. Value is expected to be <c>null</c> if the <c>Filters</c> property is set (So it is a combined filter).
        /// </summary>
        [DataMember(Name = "value")]
        public object? Value { get; set; }

        /// <summary>
        /// Gets or sets the filtering operator. Value is expected to be <c>null</c> if the <c>Filters</c> property is set (So it is a combined filter).
        /// </summary>
        [DataMember(Name = "operator")]
        public string? Operator { get; set; }

        /// <summary>
        /// (Values: "and"/"or") Gets or sets the filtering logic. Should not br <c>null</c> if <c>Filters</c> is set.
        /// </summary>
        [DataMember(Name = "logic")]
        public string? Logic { get; set; }

        /// <summary>
        /// Gets or sets the child filter expressions. Set to <c>null</c> if there are no child expressions.
        /// </summary>
        [DataMember(Name = "filters")]
        public IEnumerable<Filter>? Filters { get; set; }

        /// <summary>
        /// Mapping of filtering operators to Dynamic Linq
        /// </summary>
        private static readonly IDictionary<string, string> operators = new Dictionary<string, string>
        {
            {"eq", "="},
            {"neq", "!="},
            {"lt", "<"},
            {"lte", "<="},
            {"gt", ">"},
            {"gte", ">="},
            {"startswith", "StartsWith"},
            {"endswith", "EndsWith"},
            {"contains", "Contains"},
            {"doesnotcontain", "Contains"}
        };

        /// <summary>
        /// Get a flattened list of all child filter expressions.
        /// </summary>
        public IList<Filter> GetFlat()
        {
            var filters = new List<Filter>();

            Flatten(filters);

            return filters;
        }

        private void Flatten(IList<Filter> filters)
        {
            if (Filters != null && Filters.Any())
            {
                foreach (Filter filter in Filters)
                {
                    filters.Add(filter);
                    filter.Flatten(filters);
                }
            }
            else
            {
                filters.Add(this);
            }
        }

        /// <summary>
        /// Converts the filter expression to a predicate suitable for Dynamic Linq e.g. "Field1 = @1 and Field2.Contains(@2)"
        /// </summary>
        /// <param name="filters">A list of flattened filters.</param>
        public string ToExpression(IList<Filter> filters)
        {
            if (Filters != null && Filters.Any())
            {
                return "(" + String.Join(" " + Logic + " ", Filters.Select(filter => filter.ToExpression(filters)).ToArray()) + ")";
            }

            int index = filters.IndexOf(this);

            string comparison = operators[Operator];

            if (Operator == "doesnotcontain")
            {
                return String.Format("!{0}.{1}(@{2})", Field, comparison, index);
            }

            if (comparison == "StartsWith" || comparison == "EndsWith" || comparison == "Contains")
            {
                return String.Format("{0}.{1}(@{2})", Field, comparison, index);
            }

            return String.Format("{0} {1} @{2}", Field, comparison, index);
        }
    }
}
