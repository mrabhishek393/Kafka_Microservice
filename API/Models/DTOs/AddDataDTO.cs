using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class AddDataDTO
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public List<Guid>? Entitlements { get; set; }
    }
}
