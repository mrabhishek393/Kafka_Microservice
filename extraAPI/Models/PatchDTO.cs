using System.ComponentModel.DataAnnotations;

namespace extraAPI.Models
{
    public class PatchDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public List<Guid>? Entitlements { get; set; }
    }
}
