using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

using NorthwindSalesAnalysis.Models.ViewModels;
using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindSalesAnalysis.Services
{
    public class ExportService
    {
        public async Task<byte[]> ExportToPdfAsync<T>(T data, string reportTitle)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                document.Add(new Paragraph($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}"));
                document.Add(new Paragraph("\n"));

                // Implementar lógica específica para cada tipo de informe
                if (data is CustomerSalesViewModel customerData)
                {
                    AddCustomerSalesToPdf(document, customerData);
                }
                else if (data is ProductSalesViewModel productData)
                {
                    AddProductSalesToPdf(document, productData);
                }
                else if (data is CategorySalesViewModel categoryData)
                {
                    AddCategorySalesToPdf(document, categoryData);
                }
                else if (data is SalesTrendViewModel trendData)
                {
                    AddSalesTrendsToPdf(document, trendData);
                }

                document.Close();
                return ms.ToArray();
            }
        }

        public async Task<byte[]> ExportToExcelAsync<T>(T data, string reportTitle)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(reportTitle);

                // Añadir título del informe
                worksheet.Cells[1, 1].Value = reportTitle;
                worksheet.Cells[1, 1, 1, 5].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Size = 16;

                worksheet.Cells[2, 1].Value = $"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}";
                worksheet.Cells[2, 1, 2, 5].Merge = true;

                // Implementar lógica específica para cada tipo de informe
                if (data is CustomerSalesViewModel customerData)
                {
                    AddCustomerSalesToExcel(worksheet, customerData);
                }
                else if (data is ProductSalesViewModel productData)
                {
                    AddProductSalesToExcel(worksheet, productData);
                }
                else if (data is CategorySalesViewModel categoryData)
                {
                    AddCategorySalesToExcel(worksheet, categoryData);
                }
                else if (data is SalesTrendViewModel trendData)
                {
                    AddSalesTrendsToExcel(worksheet, trendData);
                }

                // Ajustar ancho de columnas
                worksheet.Cells.AutoFitColumns();

                return await package.GetAsByteArrayAsync();
            }
        }

        #region PDF Export Methods
        private void AddCustomerSalesToPdf(Document document, CustomerSalesViewModel data)
        {
            document.Add(new Paragraph($"Período: {data.StartDate?.ToString("dd/MM/yyyy")} - {data.EndDate?.ToString("dd/MM/yyyy")}"));

            if (data.CategoryId.HasValue)
            {
                var category = data.AvailableCategories.FirstOrDefault(c => c.CategoryId == data.CategoryId.Value);
                document.Add(new Paragraph($"Categoría: {category?.CategoryName ?? "No especificada"}"));
            }

            document.Add(new Paragraph("\n"));

            Table table = new Table(5).UseAllAvailableWidth();
            table.AddHeaderCell("Cliente ID");
            table.AddHeaderCell("Nombre del Cliente");
            table.AddHeaderCell("Total Pedidos");
            table.AddHeaderCell("Total Gastado");
            table.AddHeaderCell("Valor Promedio");

            foreach (var customer in data.CustomerSales)
            {
                table.AddCell(customer.CustomerId);
                table.AddCell(customer.CustomerName);
                table.AddCell(customer.TotalOrders.ToString());
                table.AddCell($"${customer.TotalSpent:N2}");
                table.AddCell($"${customer.AverageOrderValue:N2}");
            }

            document.Add(table);
        }

        private void AddProductSalesToPdf(Document document, ProductSalesViewModel data)
        {
            document.Add(new Paragraph($"Período: {data.StartDate?.ToString("dd/MM/yyyy")} - {data.EndDate?.ToString("dd/MM/yyyy")}"));

            if (data.CategoryId.HasValue)
            {
                var category = data.AvailableCategories.FirstOrDefault(c => c.CategoryId == data.CategoryId.Value);
                document.Add(new Paragraph($"Categoría: {category?.CategoryName ?? "No especificada"}"));
            }

            document.Add(new Paragraph("\n"));

            Table table = new Table(5).UseAllAvailableWidth();
            table.AddHeaderCell("Producto");
            table.AddHeaderCell("Categoría");
            table.AddHeaderCell("Precio Unitario");
            table.AddHeaderCell("Cantidad Vendida");
            table.AddHeaderCell("Ingresos Totales");

            foreach (var product in data.ProductSales)
            {
                table.AddCell(product.ProductName);
                table.AddCell(product.CategoryName);
                table.AddCell($"${product.UnitPrice:N2}");
                table.AddCell(product.TotalQuantity.ToString());
                table.AddCell($"${product.TotalRevenue:N2}");
            }

            document.Add(table);
        }

        private void AddCategorySalesToPdf(Document document, CategorySalesViewModel data)
        {
            document.Add(new Paragraph($"Período: {data.StartDate?.ToString("dd/MM/yyyy")} - {data.EndDate?.ToString("dd/MM/yyyy")}"));
            document.Add(new Paragraph("\n"));

            Table table = new Table(4).UseAllAvailableWidth();
            table.AddHeaderCell("Categoría");
            table.AddHeaderCell("Productos Vendidos");
            table.AddHeaderCell("Pedidos");
            table.AddHeaderCell("Ingresos Totales");

            foreach (var category in data.CategorySales)
            {
                table.AddCell(category.CategoryName);
                table.AddCell(category.ProductsSold.ToString());
                table.AddCell(category.TotalOrders.ToString());
                table.AddCell($"${category.TotalRevenue:N2}");
            }

            document.Add(table);
        }

        private void AddSalesTrendsToPdf(Document document, SalesTrendViewModel data)
        {
            document.Add(new Paragraph($"Período: {data.StartDate?.ToString("dd/MM/yyyy")} - {data.EndDate?.ToString("dd/MM/yyyy")}"));
            document.Add(new Paragraph($"Agrupación: {(data.Period == "monthly" ? "Mensual" : "Trimestral")}"));
            document.Add(new Paragraph("\n"));

            Table table = new Table(4).UseAllAvailableWidth();
            table.AddHeaderCell("Período");
            table.AddHeaderCell("Pedidos");
            table.AddHeaderCell("Ventas Totales");
            table.AddHeaderCell("Valor Promedio");

            foreach (var trend in data.SalesTrends)
            {
                table.AddCell(trend.Period);
                table.AddCell(trend.OrderCount.ToString());
                table.AddCell($"${trend.TotalSales:N2}");
                table.AddCell($"${trend.AverageOrderValue:N2}");
            }

            document.Add(table);
        }
        #endregion

        #region Excel Export Methods
        private void AddCustomerSalesToExcel(ExcelWorksheet worksheet, CustomerSalesViewModel data)
        {
            worksheet.Cells[4, 1].Value = $"Período: {data.StartDate?.ToString("dd/MM/yyyy")} - {data.EndDate?.ToString("dd/MM/yyyy")}";

            if (data.CategoryId.HasValue)
            {
                var category = data.AvailableCategories.FirstOrDefault(c => c.CategoryId == data.CategoryId.Value);
                worksheet.Cells[5, 1].Value = $"Categoría: {category?.CategoryName ?? "No especificada"}";
            }

            // Encabezados
            worksheet.Cells[7, 1].Value = "Cliente ID";
            worksheet.Cells[7, 2].Value = "Nombre del Cliente";
            worksheet.Cells[7, 3].Value = "Total Pedidos";
            worksheet.Cells[7, 4].Value = "Total Gastado";
            worksheet.Cells[7, 5].Value = "Valor Promedio";

            // Formato de encabezados
            var headerRange = worksheet.Cells[7, 1, 7, 5];
            headerRange.Style.Font.Bold = true;

            // Datos
            int row = 8;
            foreach (var customer in data.CustomerSales)
            {
                worksheet.Cells[row, 1].Value = customer.CustomerId;
                worksheet.Cells[row, 2].Value = customer.CustomerName;
                worksheet.Cells[row, 3].Value = customer.TotalOrders;
                worksheet.Cells[row, 4].Value = customer.TotalSpent;
                worksheet.Cells[row, 5].Value = customer.AverageOrderValue;

                // Formato de moneda
                worksheet.Cells[row, 4].Style.Numberformat.Format = "$#,##0.00";
                worksheet.Cells[row, 5].Style.Numberformat.Format = "$#,##0.00";

                row++;
            }
        }

        private void AddProductSalesToExcel(ExcelWorksheet worksheet, ProductSalesViewModel data)
        {
            worksheet.Cells[4, 1].Value = $"Período: {data.StartDate?.ToString("dd/MM/yyyy")} - {data.EndDate?.ToString("dd/MM/yyyy")}";

            if (data.CategoryId.HasValue)
            {
                var category = data.AvailableCategories.FirstOrDefault(c => c.CategoryId == data.CategoryId.Value);
                worksheet.Cells[5, 1].Value = $"Categoría: {category?.CategoryName ?? "No especificada"}";
            }

            // Encabezados
            worksheet.Cells[7, 1].Value = "Producto";
            worksheet.Cells[7, 2].Value = "Categoría";
            worksheet.Cells[7, 3].Value = "Precio Unitario";
            worksheet.Cells[7, 4].Value = "Cantidad Vendida";
            worksheet.Cells[7, 5].Value = "Ingresos Totales";

            // Formato de encabezados
            var headerRange = worksheet.Cells[7, 1, 7, 5];
            headerRange.Style.Font.Bold = true;

            // Datos
            int row = 8;
            foreach (var product in data.ProductSales)
            {
                worksheet.Cells[row, 1].Value = product.ProductName;
                worksheet.Cells[row, 2].Value = product.CategoryName;
                worksheet.Cells[row, 3].Value = product.UnitPrice;
                worksheet.Cells[row, 4].Value = product.TotalQuantity;
                worksheet.Cells[row, 5].Value = product.TotalRevenue;

                // Formato de moneda
                worksheet.Cells[row, 3].Style.Numberformat.Format = "$#,##0.00";
                worksheet.Cells[row, 5].Style.Numberformat.Format = "$#,##0.00";

                row++;
            }
        }

        private void AddCategorySalesToExcel(ExcelWorksheet worksheet, CategorySalesViewModel data)
        {
            worksheet.Cells[4, 1].Value = $"Período: {data.StartDate?.ToString("dd/MM/yyyy")} - {data.EndDate?.ToString("dd/MM/yyyy")}";

            // Encabezados
            worksheet.Cells[7, 1].Value = "Categoría";
            worksheet.Cells[7, 2].Value = "Productos Vendidos";
            worksheet.Cells[7, 3].Value = "Pedidos";
            worksheet.Cells[7, 4].Value = "Ingresos Totales";

            // Formato de encabezados
            var headerRange = worksheet.Cells[7, 1, 7, 4];
            headerRange.Style.Font.Bold = true;

            // Datos
            int row = 8;
            foreach (var category in data.CategorySales)
            {
                worksheet.Cells[row, 1].Value = category.CategoryName;
                worksheet.Cells[row, 2].Value = category.ProductsSold;
                worksheet.Cells[row, 3].Value = category.TotalOrders;
                worksheet.Cells[row, 4].Value = category.TotalRevenue;

                // Formato de moneda
                worksheet.Cells[row, 4].Style.Numberformat.Format = "$#,##0.00";

                row++;
            }
        }

        private void AddSalesTrendsToExcel(ExcelWorksheet worksheet, SalesTrendViewModel data)
        {
            worksheet.Cells[4, 1].Value = $"Período: {data.StartDate?.ToString("dd/MM/yyyy")} - {data.EndDate?.ToString("dd/MM/yyyy")}";
            worksheet.Cells[5, 1].Value = $"Agrupación: {(data.Period == "monthly" ? "Mensual" : "Trimestral")}";

            // Encabezados
            worksheet.Cells[7, 1].Value = "Período";
            worksheet.Cells[7, 2].Value = "Pedidos";
            worksheet.Cells[7, 3].Value = "Ventas Totales";
            worksheet.Cells[7, 4].Value = "Valor Promedio";

            // Formato de encabezados
            var headerRange = worksheet.Cells[7, 1, 7, 4];
            headerRange.Style.Font.Bold = true;

            // Datos
            int row = 8;
            foreach (var trend in data.SalesTrends)
            {
                worksheet.Cells[row, 1].Value = trend.Period;
                worksheet.Cells[row, 2].Value = trend.OrderCount;
                worksheet.Cells[row, 3].Value = trend.TotalSales;
                worksheet.Cells[row, 4].Value = trend.AverageOrderValue;

                // Formato de moneda
                worksheet.Cells[row, 3].Style.Numberformat.Format = "$#,##0.00";
                worksheet.Cells[row, 4].Style.Numberformat.Format = "$#,##0.00";

                row++;
            }
        }
        #endregion
    }
}