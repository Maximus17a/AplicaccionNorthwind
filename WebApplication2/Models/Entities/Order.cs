using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace NorthwindSalesAnalysis.Models.Entities
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        [Column("OrderID")]
        public int OrderID { get; set; }

        [StringLength(5)]
        [Column("CustomerID")]
        public string CustomerID { get; set; }

        [Column("EmployeeID")]
        public int? EmployeeID { get; set; }

        [Column("OrderDate")]
        public DateTime? OrderDate { get; set; }

        [Column("RequiredDate")]
        public DateTime? RequiredDate { get; set; }

        [Column("ShippedDate")]
        public DateTime? ShippedDate { get; set; }

        [Column("ShipVia")]
        public int? ShipVia { get; set; }

        [Column("Freight")]
        public decimal? Freight { get; set; }

        [StringLength(40)]
        [Column("ShipName")]
        public string ShipName { get; set; }

        [StringLength(60)]
        [Column("ShipAddress")]
        public string ShipAddress { get; set; }

        [StringLength(15)]
        [Column("ShipCity")]
        public string ShipCity { get; set; }

        [StringLength(15)]
        [Column("ShipRegion")]
        public string ShipRegion { get; set; }

        [StringLength(10)]
        [Column("ShipPostalCode")]
        public string ShipPostalCode { get; set; }

        [StringLength(15)]
        [Column("ShipCountry")]
        public string ShipCountry { get; set; }

        // Propiedades de navegación
        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}