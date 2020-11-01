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
    }
}