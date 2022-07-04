using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MZSimpleDynamicLinq.Core
{
    public class LinqDataResult<T>
    {
        [DataMember(Name = "recordsTotal")]
        public int RecordsTotal { get; set; }
        [DataMember(Name = "recordsFiltered")]
        public int RecordsFiltered { get; set; }
        [DataMember(Name = "data")]
        public IEnumerable<T> Data { get; set; }
    }
}
