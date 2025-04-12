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

        [HttpGet] // Cambio a GET para soportar los enlaces directos
        public async Task<IActionResult> ExportToPdf(string reportType, DateTime? startDate, DateTime? endDate, int? categoryId, string period = "monthly")
        {
            Console.WriteLine($"Recibida solicitud de exportación a PDF para {reportType}");
            byte[] fileContents;
            string fileName;

            try
            {
                switch (reportType)
                {
                    case "customer":
                        var customerData = await _repository.GetCustomerSalesAsync(startDate, endDate, categoryId);
                        Console.WriteLine($"Datos obtenidos para exportación de clientes. Registros: {customerData.CustomerSales?.Count ?? 0}");
                        fileContents = await _exportService.ExportToPdfAsync(customerData, "Ventas por Cliente");
                        fileName = "VentasPorCliente.pdf";
                        break;
                    case "product":
                        var productData = await _repository.GetProductSalesAsync(startDate, endDate, categoryId);
                        Console.WriteLine($"Datos obtenidos para exportación de productos. Registros: {productData.ProductSales?.Count ?? 0}");
                        fileContents = await _exportService.ExportToPdfAsync(productData, "Ventas por Producto");
                        fileName = "VentasPorProducto.pdf";
                        break;
                    case "category":
                        var categoryData = await _repository.GetCategorySalesAsync(startDate, endDate);
                        Console.WriteLine($"Datos obtenidos para exportación de categorías. Registros: {categoryData.CategorySales?.Count ?? 0}");
                        fileContents = await _exportService.ExportToPdfAsync(categoryData, "Análisis por Categoría");
                        fileName = "AnalisisPorCategoria.pdf";
                        break;
                    case "trends":
                        var trendsData = await _repository.GetSalesTrendsAsync(startDate, endDate, period);
                        Console.WriteLine($"Datos obtenidos para exportación de tendencias. Registros: {trendsData.SalesTrends?.Count ?? 0}");
                        fileContents = await _exportService.ExportToPdfAsync(trendsData, "Tendencias de Ventas");
                        fileName = "TendenciasDeVentas.pdf";
                        break;
                    default:
                        return BadRequest("Tipo de informe no válido");
                }

                // Asegurar que tenemos contenido antes de devolver
                if (fileContents == null || fileContents.Length == 0)
                {
                    Console.WriteLine("Error: No se pudo generar el archivo PDF (contenido vacío)");
                    return BadRequest("No se pudo generar el archivo PDF");
                }

                Console.WriteLine($"Archivo PDF generado correctamente. Tamaño: {fileContents.Length} bytes");
                return File(fileContents, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al exportar a PDF: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Error interno: {ex.InnerException.Message}");
                }
                return StatusCode(500, "Error al generar el PDF: " + ex.Message);
            }
        }

        [HttpGet] // Cambio a GET para soportar los enlaces directos
        public async Task<IActionResult> ExportToExcel(string reportType, DateTime? startDate, DateTime? endDate, int? categoryId, string period = "monthly")
        {
            Console.WriteLine($"Recibida solicitud de exportación a Excel para {reportType}");
            byte[] fileContents;
            string fileName;

            try
            {
                switch (reportType)
                {
                    case "customer":
                        var customerData = await _repository.GetCustomerSalesAsync(startDate, endDate, categoryId);
                        Console.WriteLine($"Datos obtenidos para exportación de clientes. Registros: {customerData.CustomerSales?.Count ?? 0}");
                        fileContents = await _exportService.ExportToExcelAsync(customerData, "Ventas por Cliente");
                        fileName = "VentasPorCliente.xlsx";
                        break;
                    case "product":
                        var productData = await _repository.GetProductSalesAsync(startDate, endDate, categoryId);
                        Console.WriteLine($"Datos obtenidos para exportación de productos. Registros: {productData.ProductSales?.Count ?? 0}");
                        fileContents = await _exportService.ExportToExcelAsync(productData, "Ventas por Producto");
                        fileName = "VentasPorProducto.xlsx";
                        break;
                    case "category":
                        var categoryData = await _repository.GetCategorySalesAsync(startDate, endDate);
                        Console.WriteLine($"Datos obtenidos para exportación de categorías. Registros: {categoryData.CategorySales?.Count ?? 0}");
                        fileContents = await _exportService.ExportToExcelAsync(categoryData, "Análisis por Categoría");
                        fileName = "AnalisisPorCategoria.xlsx";
                        break;
                    case "trends":
                        var trendsData = await _repository.GetSalesTrendsAsync(startDate, endDate, period);
                        Console.WriteLine($"Datos obtenidos para exportación de tendencias. Registros: {trendsData.SalesTrends?.Count ?? 0}");
                        fileContents = await _exportService.ExportToExcelAsync(trendsData, "Tendencias de Ventas");
                        fileName = "TendenciasDeVentas.xlsx";
                        break;
                    default:
                        return BadRequest("Tipo de informe no válido");
                }

                // Asegurar que tenemos contenido antes de devolver
                if (fileContents == null || fileContents.Length == 0)
                {
                    Console.WriteLine("Error: No se pudo generar el archivo Excel (contenido vacío)");
                    return BadRequest("No se pudo generar el archivo Excel");
                }

                Console.WriteLine($"Archivo Excel generado correctamente. Tamaño: {fileContents.Length} bytes");
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al exportar a Excel: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Error interno: {ex.InnerException.Message}");
                }
                return StatusCode(500, "Error al generar el Excel: " + ex.Message);
            }
        }
    }
}