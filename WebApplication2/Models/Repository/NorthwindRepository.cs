using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NorthwindSalesAnalysis.Models.Entities;
using NorthwindSalesAnalysis.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindSalesAnalysis.Models.Repository
{
    public class NorthwindRepository : INorthwindRepository
    {
        private readonly NorthwindContext _context;
        private readonly string _connectionString;

        public NorthwindRepository(NorthwindContext context)
        {
            _context = context;
            _connectionString = _context.Database.GetConnectionString();
        }

        public async Task<CustomerSalesViewModel> GetCustomerSalesAsync(DateTime? startDate, DateTime? endDate, int? categoryId)
        {
            // Establecer fechas predeterminadas si no se proporcionan
            startDate ??= DateTime.Now.AddYears(-1);
            endDate ??= DateTime.Now;

            var customerSalesData = new List<CustomerSalesData>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = @"
                        SELECT c.CustomerID, c.CompanyName, 
                            COUNT(DISTINCT o.OrderID) AS TotalOrders,
                            SUM(od.UnitPrice * od.Quantity * (1 - od.Discount)) AS TotalSpent
                        FROM Orders o
                        JOIN Customers c ON o.CustomerID = c.CustomerID
                        JOIN [Order Details] od ON o.OrderID = od.OrderID
                        JOIN Products p ON od.ProductID = p.ProductID
                        WHERE o.OrderDate BETWEEN @startDate AND @endDate";

                    if (categoryId.HasValue)
                    {
                        sql += " AND p.CategoryID = @categoryId";
                    }

                    sql += @" 
                        GROUP BY c.CustomerID, c.CompanyName
                        ORDER BY TotalSpent DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@startDate", startDate);
                        command.Parameters.AddWithValue("@endDate", endDate);

                        if (categoryId.HasValue)
                        {
                            command.Parameters.AddWithValue("@categoryId", categoryId.Value);
                        }

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                decimal totalSpent = reader.GetDecimal(3);
                                int totalOrders = reader.GetInt32(2);

                                customerSalesData.Add(new CustomerSalesData
                                {
                                    CustomerId = reader.GetString(0),
                                    CustomerName = reader.GetString(1),
                                    TotalOrders = totalOrders,
                                    TotalSpent = totalSpent,
                                    AverageOrderValue = totalOrders > 0 ? Math.Round(totalSpent / totalOrders, 2) : 0
                                });
                            }
                        }
                    }
                }

                // Obtener las categorías disponibles para el filtro
                var categories = await GetCategoriesAsync();

                return new CustomerSalesViewModel
                {
                    CustomerSales = customerSalesData,
                    StartDate = startDate,
                    EndDate = endDate,
                    CategoryId = categoryId,
                    AvailableCategories = categories.Select(c => new CategoryOption
                    {
                        CategoryId = c.CategoryID,
                        CategoryName = c.CategoryName
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener ventas por cliente: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Error interno: {ex.InnerException.Message}");
                }

                return new CustomerSalesViewModel
                {
                    CustomerSales = new List<CustomerSalesData>(),
                    StartDate = startDate,
                    EndDate = endDate,
                    CategoryId = categoryId,
                    AvailableCategories = new List<CategoryOption>()
                };
            }
        }

        public async Task<ProductSalesViewModel> GetProductSalesAsync(DateTime? startDate, DateTime? endDate, int? categoryId)
        {
            // Establecer fechas predeterminadas si no se proporcionan
            startDate ??= DateTime.Now.AddYears(-1);
            endDate ??= DateTime.Now;

            var productSalesData = new List<ProductSalesData>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = @"
                        SELECT p.ProductID, p.ProductName, c.CategoryName, p.UnitPrice,
                            SUM(od.Quantity) AS TotalQuantity,
                            SUM(od.UnitPrice * od.Quantity * (1 - od.Discount)) AS TotalRevenue
                        FROM [Order Details] od
                        JOIN Orders o ON od.OrderID = o.OrderID
                        JOIN Products p ON od.ProductID = p.ProductID
                        JOIN Categories c ON p.CategoryID = c.CategoryID
                        WHERE o.OrderDate BETWEEN @startDate AND @endDate";

                    if (categoryId.HasValue)
                    {
                        sql += " AND p.CategoryID = @categoryId";
                    }

                    sql += @" 
                        GROUP BY p.ProductID, p.ProductName, c.CategoryName, p.UnitPrice
                        ORDER BY TotalRevenue DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@startDate", startDate);
                        command.Parameters.AddWithValue("@endDate", endDate);

                        if (categoryId.HasValue)
                        {
                            command.Parameters.AddWithValue("@categoryId", categoryId.Value);
                        }

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                productSalesData.Add(new ProductSalesData
                                {
                                    ProductId = reader.GetInt32(0),
                                    ProductName = reader.GetString(1),
                                    CategoryName = reader.GetString(2),
                                    UnitPrice = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3),
                                    TotalQuantity = reader.GetInt32(4),
                                    TotalRevenue = reader.GetDecimal(5)
                                });
                            }
                        }
                    }
                }

                // Obtener las categorías disponibles para el filtro
                var categories = await GetCategoriesAsync();

                return new ProductSalesViewModel
                {
                    ProductSales = productSalesData,
                    StartDate = startDate,
                    EndDate = endDate,
                    CategoryId = categoryId,
                    AvailableCategories = categories.Select(c => new CategoryOption
                    {
                        CategoryId = c.CategoryID,
                        CategoryName = c.CategoryName
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener ventas por producto: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Error interno: {ex.InnerException.Message}");
                }

                return new ProductSalesViewModel
                {
                    ProductSales = new List<ProductSalesData>(),
                    StartDate = startDate,
                    EndDate = endDate,
                    CategoryId = categoryId,
                    AvailableCategories = new List<CategoryOption>()
                };
            }
        }

        public async Task<CategorySalesViewModel> GetCategorySalesAsync(DateTime? startDate, DateTime? endDate)
        {
            // Establecer fechas predeterminadas si no se proporcionan
            startDate ??= DateTime.Now.AddYears(-1);
            endDate ??= DateTime.Now;

            var categorySalesData = new List<CategorySalesData>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = @"
                        SELECT c.CategoryID, c.CategoryName,
                            SUM(od.Quantity) AS ProductsSold,
                            SUM(od.UnitPrice * od.Quantity * (1 - od.Discount)) AS TotalRevenue,
                            COUNT(DISTINCT o.OrderID) AS TotalOrders
                        FROM [Order Details] od
                        JOIN Orders o ON od.OrderID = o.OrderID
                        JOIN Products p ON od.ProductID = p.ProductID
                        JOIN Categories c ON p.CategoryID = c.CategoryID
                        WHERE o.OrderDate BETWEEN @startDate AND @endDate
                        GROUP BY c.CategoryID, c.CategoryName
                        ORDER BY TotalRevenue DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@startDate", startDate);
                        command.Parameters.AddWithValue("@endDate", endDate);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                categorySalesData.Add(new CategorySalesData
                                {
                                    CategoryId = reader.GetInt32(0),
                                    CategoryName = reader.GetString(1),
                                    ProductsSold = reader.GetInt32(2),
                                    TotalRevenue = reader.GetDecimal(3),
                                    TotalOrders = reader.GetInt32(4)
                                });
                            }
                        }
                    }
                }

                return new CategorySalesViewModel
                {
                    CategorySales = categorySalesData,
                    StartDate = startDate,
                    EndDate = endDate
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener ventas por categoría: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Error interno: {ex.InnerException.Message}");
                }

                return new CategorySalesViewModel
                {
                    CategorySales = new List<CategorySalesData>(),
                    StartDate = startDate,
                    EndDate = endDate
                };
            }
        }

        public async Task<SalesTrendViewModel> GetSalesTrendsAsync(DateTime? startDate, DateTime? endDate, string period = "monthly")
        {
            // Establecer fechas predeterminadas si no se proporcionan
            startDate ??= DateTime.Now.AddYears(-1);
            endDate ??= DateTime.Now;

            var salesData = new List<SalesTrendData>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql;
                    if (period.ToLower() == "monthly")
                    {
                        sql = @"
                            SELECT 
                                YEAR(o.OrderDate) AS Year,
                                MONTH(o.OrderDate) AS Month,
                                COUNT(DISTINCT o.OrderID) AS OrderCount,
                                SUM(od.UnitPrice * od.Quantity * (1 - od.Discount)) AS TotalSales
                            FROM Orders o
                            JOIN [Order Details] od ON o.OrderID = od.OrderID
                            WHERE o.OrderDate BETWEEN @startDate AND @endDate
                            GROUP BY YEAR(o.OrderDate), MONTH(o.OrderDate)
                            ORDER BY YEAR(o.OrderDate), MONTH(o.OrderDate)";
                    }
                    else
                    {
                        sql = @"
                            SELECT 
                                YEAR(o.OrderDate) AS Year,
                                (MONTH(o.OrderDate) - 1) / 3 + 1 AS Quarter,
                                COUNT(DISTINCT o.OrderID) AS OrderCount,
                                SUM(od.UnitPrice * od.Quantity * (1 - od.Discount)) AS TotalSales
                            FROM Orders o
                            JOIN [Order Details] od ON o.OrderID = od.OrderID
                            WHERE o.OrderDate BETWEEN @startDate AND @endDate
                            GROUP BY YEAR(o.OrderDate), (MONTH(o.OrderDate) - 1) / 3 + 1
                            ORDER BY YEAR(o.OrderDate), (MONTH(o.OrderDate) - 1) / 3 + 1";
                    }

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@startDate", startDate);
                        command.Parameters.AddWithValue("@endDate", endDate);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int year = reader.GetInt32(0);
                                int timeUnit = reader.GetInt32(1);
                                int orderCount = reader.GetInt32(2);
                                decimal totalSales = reader.GetDecimal(3);

                                DateTime date;
                                string periodText;

                                if (period.ToLower() == "monthly")
                                {
                                    date = new DateTime(year, timeUnit, 1);
                                    periodText = date.ToString("MMM yyyy", CultureInfo.CurrentCulture);
                                }
                                else
                                {
                                    date = new DateTime(year, (timeUnit - 1) * 3 + 1, 1);
                                    periodText = $"Q{timeUnit} {year}";
                                }

                                salesData.Add(new SalesTrendData
                                {
                                    Period = periodText,
                                    Date = date,
                                    OrderCount = orderCount,
                                    TotalSales = totalSales,
                                    AverageOrderValue = orderCount > 0 ? Math.Round(totalSales / orderCount, 2) : 0
                                });
                            }
                        }
                    }
                }

                return new SalesTrendViewModel
                {
                    SalesTrends = salesData,
                    StartDate = startDate,
                    EndDate = endDate,
                    Period = period
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener tendencias de ventas: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Error interno: {ex.InnerException.Message}");
                }

                return new SalesTrendViewModel
                {
                    SalesTrends = new List<SalesTrendData>(),
                    StartDate = startDate,
                    EndDate = endDate,
                    Period = period
                };
            }
        }

        public async Task<List<Customer>> GetCustomersAsync()
        {
            var customers = new List<Customer>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT CustomerID, CompanyName, ContactName, ContactTitle, Address, City, Region, PostalCode, Country, Phone, Fax FROM Customers ORDER BY CompanyName";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                customers.Add(new Customer
                                {
                                    CustomerID = reader.GetString(0),
                                    CompanyName = reader.GetString(1),
                                    ContactName = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    ContactTitle = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    Address = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    City = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    Region = reader.IsDBNull(6) ? null : reader.GetString(6),
                                    PostalCode = reader.IsDBNull(7) ? null : reader.GetString(7),
                                    Country = reader.IsDBNull(8) ? null : reader.GetString(8),
                                    Phone = reader.IsDBNull(9) ? null : reader.GetString(9),
                                    Fax = reader.IsDBNull(10) ? null : reader.GetString(10)
                                });
                            }
                        }
                    }
                }

                return customers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener clientes: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Error interno: {ex.InnerException.Message}");
                }

                return new List<Customer>();
            }
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var categories = new List<Category>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT CategoryID, CategoryName, Description, Picture FROM Categories ORDER BY CategoryName";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                categories.Add(new Category
                                {
                                    CategoryID = reader.GetInt32(0),
                                    CategoryName = reader.GetString(1),
                                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    Picture = reader.IsDBNull(3) ? null : (byte[])reader.GetValue(3)
                                });
                            }
                        }
                    }
                }

                return categories;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener categorías: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Error interno: {ex.InnerException.Message}");
                }

                return new List<Category>();
            }
        }

        public async Task<List<Product>> GetProductsAsync(int? categoryId)
        {
            var products = new List<Product>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = @"
                        SELECT ProductID, ProductName, SupplierID, CategoryID, QuantityPerUnit, 
                               UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued 
                        FROM Products";

                    if (categoryId.HasValue)
                    {
                        sql += " WHERE CategoryID = @categoryId";
                    }

                    sql += " ORDER BY ProductName";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        if (categoryId.HasValue)
                        {
                            command.Parameters.AddWithValue("@categoryId", categoryId.Value);
                        }

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                products.Add(new Product
                                {
                                    ProductID = reader.GetInt32(0),
                                    ProductName = reader.GetString(1),
                                    SupplierID = reader.IsDBNull(2) ? null : (int?)reader.GetInt32(2),
                                    CategoryID = reader.IsDBNull(3) ? null : (int?)reader.GetInt32(3),
                                    QuantityPerUnit = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    UnitPrice = reader.IsDBNull(5) ? null : (decimal?)reader.GetDecimal(5),
                                    UnitsInStock = reader.IsDBNull(6) ? null : (short?)reader.GetInt16(6),
                                    UnitsOnOrder = reader.IsDBNull(7) ? null : (short?)reader.GetInt16(7),
                                    ReorderLevel = reader.IsDBNull(8) ? null : (short?)reader.GetInt16(8),
                                    Discontinued = reader.GetBoolean(9)
                                });
                            }
                        }
                    }
                }

                return products;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener productos: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Error interno: {ex.InnerException.Message}");
                }

                return new List<Product>();
            }
        }

        public async Task<Usuario> GetUsuarioByNombreAsync(string nombre)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("SELECT ID_Usuario, Nombre, Apellido, Password FROM dbo.Usuarios WHERE Nombre = @nombre", connection))
                    {
                        command.Parameters.AddWithValue("@nombre", nombre);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Usuario
                                {
                                    IdUsuario = reader.GetInt32(0),
                                    Nombre = reader.GetString(1),
                                    Apellido = reader.GetString(2),
                                    Password = reader.GetString(3)
                                };
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar usuario: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Error interno: {ex.InnerException.Message}");
                }

                return null;
            }
        }

        public async Task<bool> CreateUsuarioAsync(Usuario usuario)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("INSERT INTO dbo.Usuarios (Nombre, Apellido, Password) VALUES (@nombre, @apellido, @password)", connection))
                    {
                        command.Parameters.AddWithValue("@nombre", usuario.Nombre);
                        command.Parameters.AddWithValue("@apellido", usuario.Apellido);
                        command.Parameters.AddWithValue("@password", usuario.Password);

                        var result = await command.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear usuario: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Error interno: {ex.InnerException.Message}");
                }

                return false;
            }
        }
    }
}