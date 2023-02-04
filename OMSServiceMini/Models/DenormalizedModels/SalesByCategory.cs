using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OMSServiceMini.Models.DenormalizedModels
{
    [Keyless]
    [Table("SalesByCategory")]
    public partial class SalesByCategory
    {
        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal Sales { get; set; }
    }
}