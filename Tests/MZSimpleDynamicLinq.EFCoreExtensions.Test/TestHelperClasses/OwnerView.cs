using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MZSimpleDynamicLinq.EFCoreExtensions.Test.TestHelperClasses
{
    public class OwnerView
    {
        public string ID { get; set; }
        public string Title { get; set; }
       
        public byte? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }
        public string? PersonTypeTitle { get; set; }
        public string? CardStateTitle { get; set; }
        public string? CardTypeTitle { get; set; }
        public Guid? LatestModificationMetadataId { get; set; }
    }
}
