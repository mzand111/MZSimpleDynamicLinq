namespace MZSimpleDynamicLinq.Core.HttpPostBaseClass
{
    public class Column
    {
        public object? Data { get; set; }
        public string? Name { get; set; }
        public bool? Searchable { get; set; }
        public bool? Orderable { get; set; }
        public Search Search { get; set; }
    }

}
