using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace extraAPI.Models
{
    public partial class GroupInfo
    {

        [Key]
        [Required]
        public Guid? Id { get; set; } = null!;
        public string? Name { get; set; }
        public List<Guid>? Entitlements { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
