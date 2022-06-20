using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class UpdateDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public List<Guid>? Entitlements { get; set; }
    }
}
