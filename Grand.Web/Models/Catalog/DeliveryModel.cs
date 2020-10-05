using Grand.Framework.Mvc.Models;

namespace Grand.Web.Models.Catalog
{
    public class DeliveryModel: BaseGrandEntityModel
    {
        public string Name { get; set; }
        public string Interval { get; set; }
        public string DeliveryTime { get; set; }
        public decimal Price { get; set; }
    }
}