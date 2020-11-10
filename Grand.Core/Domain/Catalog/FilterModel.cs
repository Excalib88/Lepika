namespace Grand.Core.Domain.Catalog
{
    public class FilterModel
    {
        public string CategoryId { get; set; }
        public bool IsGibkiy { get; set; }
        public bool IsNew { get; set; }
        public bool IsPodsvetka { get; set; }
        public bool InStock { get; set; }
        public bool IsExample { get; set; }
        public string InteriorFacade { get; set; } = "Все";
        public int PriceMore{ get; set; }
        public int PriceLess { get; set; } = 280000;
        public decimal WeightMore { get; set; }
        public decimal WeightLess { get; set; } = 10000;
        public decimal WidthMore { get; set; }
        public decimal WidthLess { get; set; } = 10000;
        public decimal HeightMore { get; set; }
        public decimal HeightLess { get; set; } = 10000;
    }
}