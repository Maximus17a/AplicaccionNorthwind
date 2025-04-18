﻿using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
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
            Console.WriteLine($"Iniciando exportación a PDF para '{reportTitle}'");
            try
            {
                // Usar un bloque using explícito para controlar el cierre del stream
                MemoryStream ms = new MemoryStream();
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                Console.WriteLine("Documento PDF creado correctamente");

                // Agregar título con configuración de fuente 
                Paragraph titleParagraph = new Paragraph(reportTitle)
                    .SetFontSize(20f);  // Usar un valor float explícito
                document.Add(titleParagraph);

                document.Add(new Paragraph($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}"));
                document.Add(new Paragraph("\n"));

                Console.WriteLine("Agregando contenido específico al PDF");

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

                // Cerrar el documento primero
                document.Close();

                // Obtener los bytes del stream antes de cerrar el writer
                byte[] bytes = ms.ToArray();

                // Cerrar el writer después de obtener los bytes
                writer.Close();

                // Cerrar el stream después de todo
                ms.Close();

                Console.WriteLine($"Documento PDF cerrado. Tamaño: {bytes.Length} bytes");
                return bytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar PDF: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Error interno: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<byte[]> ExportToExcelAsync<T>(T data, string reportTitle)
        {
            Console.WriteLine($"Iniciando exportación a Excel para '{reportTitle}'");
            try
            {
                // Configurar licencia de EPPlus
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(reportTitle);

                    Console.WriteLine("Documento Excel creado correctamente");

                    // Añadir título del informe
                    worksheet.Cells[1, 1].Value = reportTitle;
                    worksheet.Cells[1, 1, 1, 5].Merge = true;
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 1].Style.Font.Size = 16;

                    worksheet.Cells[2, 1].Value = $"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}";
                    worksheet.Cells[2, 1, 2, 5].Merge = true;

                    Console.WriteLine("Agregando contenido específico al Excel");

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

                    var bytes = await package.GetAsByteArrayAsync();
                    Console.WriteLine($"Documento Excel completado. Tamaño: {bytes.Length} bytes");
                    return bytes;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar Excel: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Error interno: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        #region PDF Export Methods
        private void AddCustomerSalesToPdf(Document document, CustomerSalesViewModel data)
        {
            try
            {
                document.Add(new Paragraph($"Período: {data.StartDate?.ToString("dd/MM/yyyy")} - {data.EndDate?.ToString("dd/MM/yyyy")}"));

                if (data.CategoryId.HasValue)
                {
                    var category = data.AvailableCategories.FirstOrDefault(c => c.CategoryId == data.CategoryId.Value);
                    document.Add(new Paragraph($"Categoría: {category?.CategoryName ?? "No especificada"}"));
                }

                document.Add(new Paragraph("\n"));

                // Crear tabla con el método correcto
                Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 3, 1, 1, 1 }))
                    .UseAllAvailableWidth();

                // Crear celdas de encabezado 
                Cell headerCell1 = new Cell().Add(new Paragraph("Cliente ID"));
                Cell headerCell2 = new Cell().Add(new Paragraph("Nombre del Cliente"));
                Cell headerCell3 = new Cell().Add(new Paragraph("Total Pedidos"));
                Cell headerCell4 = new Cell().Add(new Paragraph("Total Gastado"));
                Cell headerCell5 = new Cell().Add(new Paragraph("Valor Promedio"));

                // Aplicar estilo a las celdas de encabezado
                headerCell1.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));
                headerCell2.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));
                headerCell3.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));
                headerCell4.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));
                headerCell5.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));

                table.AddHeaderCell(headerCell1);
                table.AddHeaderCell(headerCell2);
                table.AddHeaderCell(headerCell3);
                table.AddHeaderCell(headerCell4);
                table.AddHeaderCell(headerCell5);

                foreach (var customer in data.CustomerSales)
                {
                    table.AddCell(customer.CustomerId);
                    table.AddCell(customer.CustomerName);
                    table.AddCell(customer.TotalOrders.ToString());
                    table.AddCell($"${customer.TotalSpent:N2}");
                    table.AddCell($"${customer.AverageOrderValue:N2}");
                }

                document.Add(table);
                Console.WriteLine($"PDF de ventas por cliente generado con {data.CustomerSales.Count} registros");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AddCustomerSalesToPdf: {ex.Message}");
                throw;
            }
        }

        private void AddProductSalesToPdf(Document document, ProductSalesViewModel data)
        {
            try
            {
                document.Add(new Paragraph($"Período: {data.StartDate?.ToString("dd/MM/yyyy")} - {data.EndDate?.ToString("dd/MM/yyyy")}"));

                if (data.CategoryId.HasValue)
                {
                    var category = data.AvailableCategories.FirstOrDefault(c => c.CategoryId == data.CategoryId.Value);
                    document.Add(new Paragraph($"Categoría: {category?.CategoryName ?? "No especificada"}"));
                }

                document.Add(new Paragraph("\n"));

                // Usar el método correcto para crear una tabla
                Table table = new Table(UnitValue.CreatePercentArray(new float[] { 3, 2, 1, 1, 1 }))
                    .UseAllAvailableWidth();

                // Crear celdas de encabezado
                Cell headerCell1 = new Cell().Add(new Paragraph("Producto"));
                Cell headerCell2 = new Cell().Add(new Paragraph("Categoría"));
                Cell headerCell3 = new Cell().Add(new Paragraph("Precio Unitario"));
                Cell headerCell4 = new Cell().Add(new Paragraph("Cantidad Vendida"));
                Cell headerCell5 = new Cell().Add(new Paragraph("Ingresos Totales"));

                // Aplicar estilo a las celdas de encabezado
                headerCell1.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));
                headerCell2.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));
                headerCell3.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));
                headerCell4.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));
                headerCell5.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));

                table.AddHeaderCell(headerCell1);
                table.AddHeaderCell(headerCell2);
                table.AddHeaderCell(headerCell3);
                table.AddHeaderCell(headerCell4);
                table.AddHeaderCell(headerCell5);

                foreach (var product in data.ProductSales)
                {
                    table.AddCell(product.ProductName);
                    table.AddCell(product.CategoryName);
                    table.AddCell($"${product.UnitPrice:N2}");
                    table.AddCell(product.TotalQuantity.ToString());
                    table.AddCell($"${product.TotalRevenue:N2}");
                }

                document.Add(table);
                Console.WriteLine($"PDF de ventas por producto generado con {data.ProductSales.Count} registros");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AddProductSalesToPdf: {ex.Message}");
                throw;
            }
        }

        private void AddCategorySalesToPdf(Document document, CategorySalesViewModel data)
        {
            try
            {
                document.Add(new Paragraph($"Período: {data.StartDate?.ToString("dd/MM/yyyy")} - {data.EndDate?.ToString("dd/MM/yyyy")}"));
                document.Add(new Paragraph("\n"));

                // Usar el método correcto para crear una tabla
                Table table = new Table(UnitValue.CreatePercentArray(new float[] { 3, 1, 1, 2 }))
                    .UseAllAvailableWidth();

                // Crear celdas de encabezado
                Cell headerCell1 = new Cell().Add(new Paragraph("Categoría"));
                Cell headerCell2 = new Cell().Add(new Paragraph("Productos Vendidos"));
                Cell headerCell3 = new Cell().Add(new Paragraph("Pedidos"));
                Cell headerCell4 = new Cell().Add(new Paragraph("Ingresos Totales"));

                // Aplicar estilo a las celdas de encabezado
                headerCell1.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));
                headerCell2.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));
                headerCell3.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));
                headerCell4.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));

                table.AddHeaderCell(headerCell1);
                table.AddHeaderCell(headerCell2);
                table.AddHeaderCell(headerCell3);
                table.AddHeaderCell(headerCell4);

                foreach (var category in data.CategorySales)
                {
                    table.AddCell(category.CategoryName);
                    table.AddCell(category.ProductsSold.ToString());
                    table.AddCell(category.TotalOrders.ToString());
                    table.AddCell($"${category.TotalRevenue:N2}");
                }

                document.Add(table);
                Console.WriteLine($"PDF de ventas por categoría generado con {data.CategorySales.Count} registros");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AddCategorySalesToPdf: {ex.Message}");
                throw;
            }
        }

        private void AddSalesTrendsToPdf(Document document, SalesTrendViewModel data)
        {
            try
            {
                document.Add(new Paragraph($"Período: {data.StartDate?.ToString("dd/MM/yyyy")} - {data.EndDate?.ToString("dd/MM/yyyy")}"));
                document.Add(new Paragraph($"Agrupación: {(data.Period == "monthly" ? "Mensual" : "Trimestral")}"));
                document.Add(new Paragraph("\n"));

                // Usar el método correcto para crear una tabla
                Table table = new Table(UnitValue.CreatePercentArray(new float[] { 2, 1, 2, 2 }))
                    .UseAllAvailableWidth();

                // Crear celdas de encabezado
                Cell headerCell1 = new Cell().Add(new Paragraph("Período"));
                Cell headerCell2 = new Cell().Add(new Paragraph("Pedidos"));
                Cell headerCell3 = new Cell().Add(new Paragraph("Ventas Totales"));
                Cell headerCell4 = new Cell().Add(new Paragraph("Valor Promedio"));

                // Aplicar estilo a las celdas de encabezado
                headerCell1.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));
                headerCell2.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));
                headerCell3.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));
                headerCell4.SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(240, 240, 240));

                table.AddHeaderCell(headerCell1);
                table.AddHeaderCell(headerCell2);
                table.AddHeaderCell(headerCell3);
                table.AddHeaderCell(headerCell4);

                foreach (var trend in data.SalesTrends)
                {
                    table.AddCell(trend.Period);
                    table.AddCell(trend.OrderCount.ToString());
                    table.AddCell($"${trend.TotalSales:N2}");
                    table.AddCell($"${trend.AverageOrderValue:N2}");
                }

                document.Add(table);
                Console.WriteLine($"PDF de tendencias de ventas generado con {data.SalesTrends.Count} registros");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AddSalesTrendsToPdf: {ex.Message}");
                throw;
            }
        }
        #endregion

        #region Excel Export Methods
        private void AddCustomerSalesToExcel(ExcelWorksheet worksheet, CustomerSalesViewModel data)
        {
            try
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

                Console.WriteLine($"Excel de ventas por cliente generado con {data.CustomerSales.Count} registros");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AddCustomerSalesToExcel: {ex.Message}");
                throw;
            }
        }

        private void AddProductSalesToExcel(ExcelWorksheet worksheet, ProductSalesViewModel data)
        {
            try
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

                Console.WriteLine($"Excel de ventas por producto generado con {data.ProductSales.Count} registros");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AddProductSalesToExcel: {ex.Message}");
                throw;
            }
        }

        private void AddCategorySalesToExcel(ExcelWorksheet worksheet, CategorySalesViewModel data)
        {
            try
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

                Console.WriteLine($"Excel de ventas por categoría generado con {data.CategorySales.Count} registros");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AddCategorySalesToExcel: {ex.Message}");
                throw;
            }
        }

        private void AddSalesTrendsToExcel(ExcelWorksheet worksheet, SalesTrendViewModel data)
        {
            try
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

                Console.WriteLine($"Excel de tendencias de ventas generado con {data.SalesTrends.Count} registros");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AddSalesTrendsToExcel: {ex.Message}");
                throw;
            }
        }
        #endregion
    }
}