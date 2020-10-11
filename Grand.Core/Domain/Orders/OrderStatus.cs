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
        NewOrder = 10,
        /// <summary>
        /// In Processing
        /// </summary>
        InProcessing = 20,
        /// <summary>
        /// Ready to payment
        /// </summary>
        ReadyToPay = 30,
        
        /// <summary>
        /// Оплачен
        /// </summary>
        Paid = 50,
        
        /// <summary>
        /// Сборка и доставка
        /// </summary>
        AssemblyAndDelivery = 60,
        
        /// <summary>
        /// Доставлен
        /// </summary>
        Delivered = 70,
        
        /// <summary>
        /// Выполнен
        /// </summary>
        Completed = 80,
        
        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled = 40
    }
}
