using NorthwindSalesAnalysis.Models.Entities;
using NorthwindSalesAnalysis.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NorthwindSalesAnalysis.Models.Repository
{
    public interface INorthwindRepository
    {
        // Métodos para obtener datos de informes
        Task<CustomerSalesViewModel> GetCustomerSalesAsync(DateTime? startDate, DateTime? endDate, int? categoryId);
        Task<ProductSalesViewModel> GetProductSalesAsync(DateTime? startDate, DateTime? endDate, int? categoryId);
        Task<CategorySalesViewModel> GetCategorySalesAsync(DateTime? startDate, DateTime? endDate);
        Task<SalesTrendViewModel> GetSalesTrendsAsync(DateTime? startDate, DateTime? endDate, string period = "monthly");

        // Métodos auxiliares para filtros
        Task<List<Customer>> GetCustomersAsync();
        Task<List<Category>> GetCategoriesAsync();
        Task<List<Product>> GetProductsAsync(int? categoryId);

        // Métodos para autenticación de usuarios
        Task<Usuario> GetUsuarioByNombreAsync(string nombre);
        Task<bool> CreateUsuarioAsync(Usuario usuario);
    }
}