using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Grand.Core.Domain.Orders
{
    /// <summary>
    /// Represents an order status enumeration
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// New order
        /// </summary>
        [Display(Name="Новый заказ")]
        NewOrder = 10,
        /// <summary>
        /// In Processing
        /// </summary>
        [Display(Name="В обработке")]
        InProcessing = 20,
        /// <summary>
        /// Ready to payment
        /// </summary>
        [Display(Name="Готов к оплате")]
        ReadyToPay = 30,
        
        /// <summary>
        /// Оплачен
        /// </summary>
        [Display(Name="Оплачен")]
        Paid = 50,
        
        /// <summary>
        /// Сборка и доставка
        /// </summary>
        [Display(Name="Сборка и доставка")]
        AssemblyAndDelivery = 60,
        
        /// <summary>
        /// Доставлен
        /// </summary>
        [Display(Name="Доставлен")]
        Delivered = 70,
        
        /// <summary>
        /// Выполнен
        /// </summary>
        [Display(Name="Выполнен")]
        Completed = 80,
        
        /// <summary>
        /// Cancelled
        /// </summary>
        [Display(Name="Отменен")]
        Cancelled = 40
    }
}
