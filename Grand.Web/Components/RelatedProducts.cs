using System.Collections.Generic;
using Grand.Core;
using Grand.Core.Caching;
using Grand.Core.Domain.Catalog;
using Grand.Framework.Components;
using Grand.Services.Catalog;
using Grand.Web.Features.Models.Products;
using Grand.Web.Infrastructure.Cache;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Grand.Web.Components
{
    public class RelatedProductsViewComponent : BaseViewComponent
    {
        #region Fields
        private readonly ICacheManager _cacheManager;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IMediator _mediator;

        private readonly CatalogSettings _catalogSettings;
        #endregion

        #region Constructors

        public RelatedProductsViewComponent(
            ICacheManager cacheManager,
            IProductService productService,
            IStoreContext storeContext,
            IMediator mediator,
            CatalogSettings catalogSettings)
        {
            _cacheManager = cacheManager;
            _productService = productService;
            _storeContext = storeContext;
            _mediator = mediator;
            _catalogSettings = catalogSettings;
        }

        #endregion

        #region Invoker

        public async Task<IViewComponentResult> InvokeAsync(string productId, int? productThumbPictureSize)
        {
            var productIds = await _cacheManager.GetAsync(string.Format(ModelCacheEventConst.PRODUCTS_RELATED_IDS_KEY, productId, _storeContext.CurrentStore.Id),
                  async () => (await _productService.GetProductById(productId)).RelatedProducts.OrderBy(x => x.DisplayOrder).Select(x => x.ProductId2).ToArray());

            if (!productIds.Any())
            {
                var product = await _productService.GetProductById(productId);
                
                if (product != null && !string.IsNullOrEmpty(product.UseWith))
                {
                    var relates = product.UseWith.Split(";");
                    var productIdsList = new List<string>();
                    foreach (var relate in relates)
                    {
                        var relateProduct = await _productService.GetByImportId(relate);
                        
                        if(relateProduct == null) continue;
                        
                        product.RelatedProducts.Add(new RelatedProduct {
                            ProductId1 = productId,
                            ProductId2 = relateProduct.Id
                        });
                        
                        productIdsList.Add(relateProduct.Id);
                    }

                    productIds = productIdsList.ToArray();
                }
            }
            
            
            //load products
            var products = await _productService.GetProductsByIds(productIds);

            var model = await _mediator.Send(new GetProductOverview {
                PreparePictureModel = true,
                PreparePriceModel = true,
                PrepareSpecificationAttributes = _catalogSettings.ShowSpecAttributeOnCatalogPages,
                ProductThumbPictureSize = productThumbPictureSize,
                Products = products
            });

            return View(model);
        }

        #endregion
    }
}
