using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindSalesAnalysis.Models.Repository;
using NorthwindSalesAnalysis.Models.ViewModels;
using NorthwindSalesAnalysis.Services;
using System;
using System.Threading.Tasks;


namespace NorthwindSalesAnalysis.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly INorthwindRepository _repository;
        private readonly ExportService _exportService;

        public ReportsController(INorthwindRepository repository, ExportService exportService)
        {
            _repository = repository;
            _exportService = exportService;
        }

        public async Task<IActionResult> CustomerSales(DateTime? startDate, DateTime? endDate, int? categoryId)
        {
            var customerSalesData = await _repository.GetCustomerSalesAsync(startDate, endDate, categoryId);
            return View(customerSalesData);
        }

        public async Task<IActionResult> ProductSales(DateTime? startDate, DateTime? endDate, int? categoryId)
        {
            var productSalesData = await _repository.GetProductSalesAsync(startDate, endDate, categoryId);
            return View(productSalesData);
        }

        public async Task<IActionResult> CategorySales(DateTime? startDate, DateTime? endDate)
        {
            var categorySalesData = await _repository.GetCategorySalesAsync(startDate, endDate);
            return View(categorySalesData);
        }

        public async Task<IActionResult> SalesTrends(DateTime? startDate, DateTime? endDate, string period = "monthly")
        {
            var salesTrendsData = await _repository.GetSalesTrendsAsync(startDate, endDate, period);
            return View(salesTrendsData);
        }

        [HttpPost]
        public async Task<IActionResult> ExportToPdf(string reportType, DateTime? startDate, DateTime? endDate, int? categoryId)
        {
            byte[] fileContents;
            string fileName;

            switch (reportType)
            {
                case "customer":
                    var customerData = await _repository.GetCustomerSalesAsync(startDate, endDate, categoryId);
                    fileContents = await _exportService.ExportToPdfAsync(customerData, "Ventas por Cliente");
                    fileName = "VentasPorCliente.pdf";
                    break;
                case "product":
                    var productData = await _repository.GetProductSalesAsync(startDate, endDate, categoryId);
                    fileContents = await _exportService.ExportToPdfAsync(productData, "Ventas por Producto");
                    fileName = "VentasPorProducto.pdf";
                    break;
                case "category":
                    var categoryData = await _repository.GetCategorySalesAsync(startDate, endDate);
                    fileContents = await _exportService.ExportToPdfAsync(categoryData, "Análisis por Categoría");
                    fileName = "AnalisisPorCategoria.pdf";
                    break;
                case "trends":
                    var trendsData = await _repository.GetSalesTrendsAsync(startDate, endDate);
                    fileContents = await _exportService.ExportToPdfAsync(trendsData, "Tendencias de Ventas");
                    fileName = "TendenciasDeVentas.pdf";
                    break;
                default:
                    return BadRequest("Tipo de informe no válido");
            }

            return File(fileContents, "application/pdf", fileName);
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel(string reportType, DateTime? startDate, DateTime? endDate, int? categoryId)
        {
            byte[] fileContents;
            string fileName;

            switch (reportType)
            {
                case "customer":
                    var customerData = await _repository.GetCustomerSalesAsync(startDate, endDate, categoryId);
                    fileContents = await _exportService.ExportToExcelAsync(customerData, "Ventas por Cliente");
                    fileName = "VentasPorCliente.xlsx";
                    break;
                case "product":
                    var productData = await _repository.GetProductSalesAsync(startDate, endDate, categoryId);
                    fileContents = await _exportService.ExportToExcelAsync(productData, "Ventas por Producto");
                    fileName = "VentasPorProducto.xlsx";
                    break;
                case "category":
                    var categoryData = await _repository.GetCategorySalesAsync(startDate, endDate);
                    fileContents = await _exportService.ExportToExcelAsync(categoryData, "Análisis por Categoría");
                    fileName = "AnalisisPorCategoria.xlsx";
                    break;
                case "trends":
                    var trendsData = await _repository.GetSalesTrendsAsync(startDate, endDate);
                    fileContents = await _exportService.ExportToExcelAsync(trendsData, "Tendencias de Ventas");
                    fileName = "TendenciasDeVentas.xlsx";
                    break;
                default:
                    return BadRequest("Tipo de informe no válido");
            }

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}