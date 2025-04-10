using System;
using System.Collections.Generic;

namespace NorthwindSalesAnalysis.Models.ViewModels
{
    public class CustomerSalesViewModel
    {
        public List<CustomerSalesData> CustomerSales { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CategoryId { get; set; }
        public List<CategoryOption> AvailableCategories { get; set; }
    }

    public class CustomerSalesData
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class CategoryOption
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}