using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MZSimpleDynamicLinq.Core
{
    [DataContract]
    public class Sort
    {
        /// <summary>
        /// Gets or sets the name of the property wich soring should be done against.
        /// </summary>
        [DataMember(Name = "field")]
        public string Field { get; set; }

        /// <summary>
        /// Gets or sets the sorting direction: "asc" or "desc".
        /// </summary>
        [DataMember(Name = "dir")]
        public string Dir { get; set; }

        /// <summary>
        /// Converts to form required by Dynamic Linq e.g. "Field1 desc"
        /// </summary>
        public string ToExpression()
        {
            return Field + " " + Dir;
        }
    }
}
