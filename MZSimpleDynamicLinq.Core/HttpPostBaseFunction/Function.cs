using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MZSimpleDynamicLinq.Core.HttpPostBaseFunction
{
    public static class Function
    {
        public static (int, int, int, string) GetSearchValue(string json)
        {
            JObject jObject = JObject.Parse(json);
            int draw = (int)jObject["draw"];
            int start = (int)jObject["start"];
            int length = (int)jObject["length"];
            string searchValue = (string)jObject["search"]["value"];
            return (draw, start, length, searchValue?.ToString());
        }
    }
}
