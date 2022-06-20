using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
    public partial class GroupModel
    {
        [Key]
        [Required]
        public Guid? Id { get; set; } = null!;
        public string? Name { get; set; }
        public List<Guid>? Entitlements { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
