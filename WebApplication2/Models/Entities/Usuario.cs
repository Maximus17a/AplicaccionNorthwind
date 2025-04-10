using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NorthwindSalesAnalysis.Models.Entities
{
    [Table("Usuarios", Schema = "dbo")]
    public class Usuario
    {
        [Key]
        [Column("ID_Usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Nombre")]
        public string Nombre { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Apellido")]
        public string Apellido { get; set; }

        [Required]
        [StringLength(255)]
        [Column("Password")]
        public string Password { get; set; }
    }
}