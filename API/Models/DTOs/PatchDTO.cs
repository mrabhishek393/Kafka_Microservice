using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class PatchDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public List<Guid>? Entitlements { get; set; }
    }
}
