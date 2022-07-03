using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MZSimpleDynamicLinq.Core
{
	[DataContract(Name = "aggregate")]
	public class Aggregator
	{
		/// <summary>
		/// Gets or sets the name of the aggregated field (property).
		/// </summary>
		[DataMember(Name = "field")]
		public string Field { get; set; }

		/// <summary>
		/// Gets or sets the aggregate.Values: "min"/"max"/"average"/"sum"/"count"
		/// </summary>
		[DataMember(Name = "aggregate")]
		public string Aggregate { get; set; }

	
	}
}
