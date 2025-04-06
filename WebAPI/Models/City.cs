using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class City
    {
       public int Id { get; set; }
       public string Name { get; set; } = string.Empty;
       [Required]

       public string Country { get; set; } = string.Empty;
       public DateTime LastUpdated { get; set; } = DateTime.Now;

       public int LastUpdatedBy { get; set; } = 0;
    }
}
