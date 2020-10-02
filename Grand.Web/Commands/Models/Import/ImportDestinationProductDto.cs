using Grand.Api.DTOs.Catalog;

namespace Grand.Web.Commands.Models.Import
{
    public class ImportDestinationProductDto
    {
        public ProductDto Product { get; set; }
        public CategoryDto Category { get; set; }
        public ManufacturerDto Manufacturer { get; set; }
    }
}