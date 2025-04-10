using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NorthwindSalesAnalysis.Models.Entities
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [Column("ProductID")]
        public int ProductID { get; set; }

        [Required]
        [StringLength(40)]
        [Column("ProductName")]
        public string ProductName { get; set; }

        [Column("SupplierID")]
        public int? SupplierID { get; set; }

        [Column("CategoryID")]
        public int? CategoryID { get; set; }

        [StringLength(20)]
        [Column("QuantityPerUnit")]
        public string QuantityPerUnit { get; set; }

        [Column("UnitPrice")]
        public decimal? UnitPrice { get; set; }

        [Column("UnitsInStock")]
        public short? UnitsInStock { get; set; }

        [Column("UnitsOnOrder")]
        public short? UnitsOnOrder { get; set; }

        [Column("ReorderLevel")]
        public short? ReorderLevel { get; set; }

        [Column("Discontinued")]
        public bool Discontinued { get; set; }

        // Propiedades de navegación
        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}