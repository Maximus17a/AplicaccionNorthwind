using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace NorthwindSalesAnalysis.Models.Entities
{
    [Table("Customers")]
    public class Customer
    {
        [Key]
        [StringLength(5)]
        [Column("CustomerID")]
        public string CustomerID { get; set; }

        [Required]
        [StringLength(40)]
        [Column("CompanyName")]
        public string CompanyName { get; set; }

        [StringLength(30)]
        [Column("ContactName")]
        public string ContactName { get; set; }

        [StringLength(30)]
        [Column("ContactTitle")]
        public string ContactTitle { get; set; }

        [StringLength(60)]
        [Column("Address")]
        public string Address { get; set; }

        [StringLength(15)]
        [Column("City")]
        public string City { get; set; }

        [StringLength(15)]
        [Column("Region")]
        public string Region { get; set; }

        [StringLength(10)]
        [Column("PostalCode")]
        public string PostalCode { get; set; }

        [StringLength(15)]
        [Column("Country")]
        public string Country { get; set; }

        [StringLength(24)]
        [Column("Phone")]
        public string Phone { get; set; }

        [StringLength(24)]
        [Column("Fax")]
        public string Fax { get; set; }

        // Propiedad de navegación para pedidos
        public virtual ICollection<Order> Orders { get; set; }
    }
}