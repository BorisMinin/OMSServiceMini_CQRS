using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OMSServiceMini.Models.DenormalizedModels
{
    [Keyless]
    [Table("OrdersByCountry")]
    public partial class OrdersByCountry
    {
        [Required]
        [StringLength(15)]
        public string CountryName { get; set; }
        public int OrdersCount { get; set; }
    }
}