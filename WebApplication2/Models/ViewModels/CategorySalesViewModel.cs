using System;
using System.Collections.Generic;

namespace NorthwindSalesAnalysis.Models.ViewModels
{
    public class CategorySalesViewModel
    {
        public List<CategorySalesData> CategorySales { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class CategorySalesData
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
    }
}