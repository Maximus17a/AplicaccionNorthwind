using System;
using System.Collections.Generic;

namespace NorthwindSalesAnalysis.Models.ViewModels
{
    public class SalesTrendViewModel
    {
        public List<SalesTrendData> SalesTrends { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Period { get; set; } // "monthly" o "quarterly"
    }

    public class SalesTrendData
    {
        public string Period { get; set; } // "Ene 2023", "Q1 2023", etc.
        public DateTime Date { get; set; }
        public decimal TotalSales { get; set; }
        public int OrderCount { get; set; }
        public decimal AverageOrderValue { get; set; }
    }
}