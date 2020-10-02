using System.Collections.Generic;
using System.Threading.Tasks;
using Grand.Api.DTOs.Catalog;
using Grand.Web.Commands.Models.Import;

namespace Grand.Web.Services
{
    public interface IImportService
    {
        Task<ImportSource> Deserialize();
        Task<List<ProductDto>> MapRange(ImportSource importSourceProducts);
    }
}