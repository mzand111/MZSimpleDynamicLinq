using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MZSimpleDynamicLinq.Core
{
    public class LinqDataRequest
    {
        /// Specifies how many items to take.
        /// </summary>
        public int Take { get; set; }

        /// <summary>
        /// Specifies how many items to skip.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Specifies the requested sort order.
        /// </summary>
        public IEnumerable<Sort>? Sort { get; set; }

        /// <summary>
        /// Specifies the requested filter.
        /// </summary>
        public Filter? Filter { get; set; }
    }
}
