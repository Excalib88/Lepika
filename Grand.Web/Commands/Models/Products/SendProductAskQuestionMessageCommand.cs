﻿using Grand.Core.Domain.Catalog;
using Grand.Core.Domain.Customers;
using Grand.Core.Domain.Localization;
using Grand.Core.Domain.Stores;
using Grand.Web.Models.Catalog;
using MediatR;

namespace Grand.Web.Commands.Models.Products
{
    public class SendProductAskQuestionMessageCommand : IRequest<bool>
    {
        public Customer Customer { get; set; }
        public Store Store { get; set; }
        public Language Language { get; set; }
        public Product Product { get; set; }
        public ProductAskQuestionModel Model { get; set; }
    }
}
