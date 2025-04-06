using System.ComponentModel.DataAnnotations;


namespace WebAPI.Dtos
{
    public class CityDto
    {
        public int Id { get; set; }

        [Required (ErrorMessage = "City name is required")]
        [StringLength(100, ErrorMessage = "City name cannot be longer than 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City name can only contain letters and spaces")]
        public string Name { get; set; } = string.Empty;

        
        [Required]
        public string Country { get; set; } = string.Empty;
    }
}