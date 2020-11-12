using System.Collections.Generic;

namespace Grand.Web.Commands.Models.Import
{
    public class ImportSource
    {
        public List<ImportSourceProductDto> ProductDto { get; set; }
        public List<ImportStock> Stock { get; set; }
    }
    
    public class ImportStock
    {
        public int Id { get; set; }
        public double Stock { get; set; }
    }
    
    public class ImportSourceProductDto
    {
        public string Analogs { get; set; }
        public string Article { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string Collection { get; set; }
        public string CutImage { get; set; }
        public string Description { get; set; }
        public int Gibkiy { get; set; }
        public double Height { get; set; }
        public int Id { get; set; }
        public string[] Image { get; set; }
        public bool IsDelete { get; set; }
        public bool IsNew { get; set; }
        public string LWHMeasure { get; set; }
        public decimal Length { get; set; }
        public int Mark { get; set; }
        public string Material { get; set; }
        public string Measure { get; set; }
        public string MinImage { get; set; }
        public string[] Model { get; set; }
        public string Name { get; set; }
        public int Obrazci { get; set; }
        public string Path { get; set; }
        public int Podsvetka { get; set; }
        public decimal Price { get; set; }
        public int QuantityInBox { get; set; }
        public string Razdel { get; set; }
        public string UseWith { get; set; }
        public double Weight { get; set; }
        public string WeightMeasure { get; set; }
        public int Width { get; set; }
    }
}