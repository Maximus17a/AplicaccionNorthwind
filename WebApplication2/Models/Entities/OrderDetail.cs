using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NorthwindSalesAnalysis.Models.Entities
{
    [Table("Order Details")]
    public class OrderDetail
    {
        [Key]
        [Column("OrderID", Order = 1)]
        public int OrderID { get; set; }

        [Key]
        [Column("ProductID", Order = 2)]
        public int ProductID { get; set; }

        [Required]
        [Column("UnitPrice")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column("Quantity")]
        public short Quantity { get; set; }

        [Required]
        [Column("Discount")]
        public float Discount { get; set; }

        // Propiedades de navegación
        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
    }
}