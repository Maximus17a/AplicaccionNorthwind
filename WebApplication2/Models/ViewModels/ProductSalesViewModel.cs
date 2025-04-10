using System;
using System.Collections.Generic;

namespace NorthwindSalesAnalysis.Models.ViewModels
{
    public class ProductSalesViewModel
    {
        public List<ProductSalesData> ProductSales { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CategoryId { get; set; }
        public List<CategoryOption> AvailableCategories { get; set; }
    }

    public class ProductSalesData
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal UnitPrice { get; set; }
    }
}