using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindSalesAnalysis.Models.Repository;
using System;
using System.Threading.Tasks;


namespace NorthwindSalesAnalysis.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly INorthwindRepository _repository;

        public ApiController(INorthwindRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("customersales")]
        public async Task<IActionResult> GetCustomerSales(DateTime? startDate, DateTime? endDate, int? categoryId)
        {
            var data = await _repository.GetCustomerSalesAsync(startDate, endDate, categoryId);
            return Ok(data);
        }

        [HttpGet("productsales")]
        public async Task<IActionResult> GetProductSales(DateTime? startDate, DateTime? endDate, int? categoryId)
        {
            var data = await _repository.GetProductSalesAsync(startDate, endDate, categoryId);
            return Ok(data);
        }

        [HttpGet("categorysales")]
        public async Task<IActionResult> GetCategorySales(DateTime? startDate, DateTime? endDate)
        {
            var data = await _repository.GetCategorySalesAsync(startDate, endDate);
            return Ok(data);
        }

        [HttpGet("salestrends")]
        public async Task<IActionResult> GetSalesTrends(DateTime? startDate, DateTime? endDate, string period = "monthly")
        {
            var data = await _repository.GetSalesTrendsAsync(startDate, endDate, period);
            return Ok(data);
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _repository.GetCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _repository.GetCategoriesAsync();
            return Ok(categories);
        }
    }
}