using System.ComponentModel.DataAnnotations;

namespace extraAPI.Models
{
    public class UpdateDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public List<Guid>? Entitlements { get; set; }

        //public bool Equals(UpdateDTO? other)
        //{
        //    return other!= null && this.GetType() == other.GetType() 
        //        && Id==other.Id && Name==other.Name && Entitlements.SequenceEqual(other.Entitlements);
        //}

        //public override bool Equals(object? obj)
        //{
        //    return Equals(obj as UpdateDTO); 
        //}
    }
}
