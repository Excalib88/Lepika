﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Grand.Api.Commands.Models.Catalog;
using Grand.Api.Commands.Models.Common;
using Grand.Api.DTOs.Catalog;
using Grand.Api.DTOs.Common;
using Grand.Core.Data;
using Grand.Core.Domain.Catalog;
using Grand.Core.Domain.Logging;
using Grand.Services.Catalog;
using Grand.Services.Logging;
using Grand.Web.Commands.Models.Import;
using Grand.Web.Extensions;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Unidecode.NET;

namespace Grand.Web.Services
{
    public class ImportService : IImportService
    {
        private readonly List<ProductDto> _productDtos;
        private readonly IMediator _mediator;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IProductService _productService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;

        public ImportService(
            IMediator mediator,
            IRepository<Category> categoryRepository,
            IRepository<Product> productRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IProductService productService,
            IManufacturerService manufacturerService,
            ICategoryService categoryService,
            ILogger logger)
        {
            _mediator = mediator;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _manufacturerRepository = manufacturerRepository;
            _productService = productService;
            _manufacturerService = manufacturerService;
            _categoryService = categoryService;
            _logger = logger;
            _productDtos = new List<ProductDto>();
        }

        public async Task<ImportSource> Deserialize()
        {
            var currentPath = Directory.GetCurrentDirectory();
            var directory = Path.Combine(currentPath, "App_Data", "Import");

            await using var reader = File.OpenRead(Path.Combine(directory, "output.json"));
            var products = await JsonSerializer.DeserializeAsync<List<ImportSourceProductDto>>(reader);

            await using var stockReader = File.OpenRead(Path.Combine(directory, "stocks.json"));
            var stocks = await JsonSerializer.DeserializeAsync<List<ImportStock>>(stockReader);

            return new ImportSource {
                Stock = stocks,
                ProductDto = products
            };
        }

        public async Task<List<ProductDto>> MapRange(ImportSource importSourceProducts)
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Import");
            var productQty = 0;
            var updatedProduct = 0;

            foreach (var sourceProduct in importSourceProducts.ProductDto)
            {
                var queryProduct = from c in _productRepository.Table
                    where c.VendorCode == sourceProduct.Article ||
                          c.Price == sourceProduct.Price &&
                          c.FullDescription == sourceProduct.Description &&
                          c.Name == sourceProduct.Name
                    select c;
                var productResult = IAsyncCursorSourceExtensions.FirstOrDefault(queryProduct);
                var isExistedProduct = productResult != null;

                if (isExistedProduct)
                {
                    if (productResult.Length == 0 && sourceProduct.Length != 0)
                    {
                        productResult.Length = sourceProduct.Length;
                    }

                    await _productRepository.UpdateAsync(productResult);
                    updatedProduct++;

                    continue;
                }

                var product = new ProductDto {
                    LWHMeasure = sourceProduct.LWHMeasure,
                    WeightMeasure = sourceProduct.WeightMeasure,
                    Name = sourceProduct.Name,
                    Width = sourceProduct.Width,
                    Height = Convert.ToDecimal(sourceProduct.Height),
                    Weight = Convert.ToDecimal(sourceProduct.Weight),
                    SeName = sourceProduct.Name.Replace(' ', '-').ToLower().Unidecode(),
                    VendorCode = sourceProduct.Article,
                    FullDescription = sourceProduct.Description,
                    ShortDescription = sourceProduct.Description,
                    Price = sourceProduct.Price,
                    Published = true,
                    DisplayStockAvailability = true,
                    DisplayStockQuantity = true,
                    VisibleIndividually = true,
                    OrderMinimumQuantity = 1,
                    OrderMaximumQuantity = 10000,
                    ProductTemplateId = "5f74eb009eb59f4650635823",
                    StockQuantity = sourceProduct.Mark == 1 ? 10000 : 0,
                    Gibkiy = sourceProduct.Gibkiy == 1,
                    Obrazci = sourceProduct.Obrazci,
                    MarkAsNew = sourceProduct.IsNew,
                    Podsvetka = sourceProduct.Podsvetka == 1,
                    QuantityInBox = sourceProduct.QuantityInBox,
                    Material = sourceProduct.Material,
                    Mark = sourceProduct.Mark,
                    Collection = sourceProduct.Collection,
                    Razdel = sourceProduct.Razdel,
                    UseWith = sourceProduct.UseWith,
                    Analogs = sourceProduct.Analogs
                };

                var queryCategory = from c in _categoryRepository.Table
                    where c.Name == sourceProduct.Category
                    select c;

                var isExistedCategory = IAsyncCursorSourceExtensions.FirstOrDefault(queryCategory) != null;
                var categoryDto = new CategoryDto();

                if (!isExistedCategory)
                {
                    categoryDto = await _mediator.Send(new AddCategoryCommand {
                        Model = new CategoryDto {
                            Name = sourceProduct.Category,
                            Published = true,
                            HideOnCatalog = false,
                            ShowOnSearchBox = false,
                            ShowOnHomePage = true,
                            CategoryTemplateId = "5f66096097db2b2da47b957d",
                            ParentCategoryId = "5f8c210ea1bd7e55c439472b"
                        }
                    });

                    product.Categories.Add(new ProductCategoryDto {
                        CategoryId = categoryDto.Id
                    });
                }
                else
                {
                    var categoryId = IAsyncCursorSourceExtensions.FirstOrDefault(queryCategory).Id;
                    categoryDto.Id = categoryId;
                    product.Categories.Add(new ProductCategoryDto {
                        CategoryId = categoryId
                    });
                }

                var pictureDtos = new List<PictureDto>();

                foreach (var image in sourceProduct.Image)
                {
                    if (image == "") continue;
                    var binary = Path.Combine(directory, "images", image).ToBytes();

                    if (binary == null)
                    {
                        continue;
                    }

                    pictureDtos.Add(await _mediator.Send(new AddPictureCommand {
                        PictureDto = new PictureDto {
                            SeoFilename = image,
                            MimeType = ("." + image.Split(".")[1]).ToMimeType(),
                            PictureBinary = binary
                        }
                    }));
                }

                var queryManufacturer = from c in _manufacturerRepository.Table
                    where c.Name == sourceProduct.Brand
                    select c;

                var isExistedManufacturer = IAsyncCursorSourceExtensions.FirstOrDefault(queryManufacturer) != null;

                var manufacturerDto = new ManufacturerDto();

                if (!isExistedManufacturer)
                {
                    manufacturerDto = await _mediator.Send(new AddManufacturerCommand {
                        Model = new ManufacturerDto {
                            Name = sourceProduct.Brand,
                            Published = true,
                            ManufacturerTemplateId = "5f66096097db2b2da47b957e"
                        }
                    });

                    product.Manufacturers.Add(new ProductManufacturerDto {
                        ManufacturerId = manufacturerDto.Id
                    });
                }
                else
                {
                    var manufacturerId = IAsyncCursorSourceExtensions.FirstOrDefault(queryManufacturer).Id;
                    manufacturerDto.Id = manufacturerId;
                    product.Manufacturers.Add(new ProductManufacturerDto {
                        ManufacturerId = manufacturerId
                    });
                }

                var productDto = await _mediator.Send(
                    new AddProductCommand {
                        Model = product
                    });

                await _categoryService.InsertProductCategory(new ProductCategory {
                    CategoryId = categoryDto.Id,
                    ProductId = productDto.Id,
                    IsFeaturedProduct = true
                });

                var productDtoExample = new ProductDto();
                var productExample = new ProductDto();
                if (product.Obrazci > 0)
                {
                    productExample = (ProductDto)product.Clone();
                    var exampleCategoryName = "Образцы";
                    var queryCategoryExample = from c in _categoryRepository.Table
                        where c.Name == exampleCategoryName
                        select c;

                    var isExistedCategoryExample =
                        IAsyncCursorSourceExtensions.FirstOrDefault(queryCategoryExample) != null;
                    var categoryDtoExample = new CategoryDto();

                    if (!isExistedCategoryExample)
                    {
                        categoryDtoExample = await _mediator.Send(new AddCategoryCommand {
                            Model = new CategoryDto {
                                Name = exampleCategoryName,
                                Published = true,
                                HideOnCatalog = true,
                                ShowOnSearchBox = false,
                                ShowOnHomePage = false,
                                CategoryTemplateId = "5f66096097db2b2da47b957d",
                                ParentCategoryId = "5f8c210ea1bd7e55c439472b"
                            }
                        });

                        var categoryForDelete = productExample.Categories.FirstOrDefault(
                            x => x.CategoryId == categoryDto.Id);
                        productExample.Categories.Remove(categoryForDelete);
                        productExample.Categories.Add(new ProductCategoryDto {
                            CategoryId = categoryDtoExample.Id
                        });
                    }
                    else
                    {
                        var categoryId = IAsyncCursorSourceExtensions.FirstOrDefault(queryCategoryExample).Id;
                        categoryDtoExample.Id = categoryId;
                        productExample.Categories.Add(new ProductCategoryDto {
                            CategoryId = categoryId
                        });
                    }

                    productDtoExample = await _mediator.Send(
                        new AddProductCommand {
                            Model = productExample
                        });

                    await _categoryService.InsertProductCategory(new ProductCategory {
                        CategoryId = categoryDtoExample.Id,
                        ProductId = productDtoExample.Id,
                        IsFeaturedProduct = true
                    });
                }

                await _manufacturerService.InsertProductManufacturer(new ProductManufacturer {
                    ManufacturerId = manufacturerDto.Id,
                    ProductId = productDto.Id,
                    IsFeaturedProduct = true
                });

                foreach (var item in pictureDtos)
                {
                    product.Pictures.Add(new ProductPictureDto {
                        MimeType = item.MimeType,
                        PictureId = item.Id
                    });

                    await _productService.InsertProductPicture(new ProductPicture {
                        PictureId = item.Id,
                        ProductId = productDto.Id
                    });

                    if (product.Obrazci > 0)
                    {
                        productExample.Pictures.Add(new ProductPictureDto {
                            MimeType = item.MimeType,
                            PictureId = item.Id
                        });

                        await _productService.InsertProductPicture(new ProductPicture {
                            PictureId = item.Id,
                            ProductId = productDtoExample.Id
                        });

                        _productDtos.Add(productDtoExample);
                    }
                }

                _productDtos.Add(productDto);

                await _logger.InsertLog(LogLevel.Information, $"Добавлен товар №{productQty}");
                productQty++;
            }

            await _logger.InsertLog(LogLevel.Information, updatedProduct.ToString());
            return _productDtos;
        }
    }
}