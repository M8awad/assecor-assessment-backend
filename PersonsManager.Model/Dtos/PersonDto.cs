using System.ComponentModel.DataAnnotations;

namespace PersonsManager.Model.Dtos
{
    public class PersonDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required and cannot be empty")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 50 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Last name is required and cannot be empty")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 50 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Zip code is required")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "City is required and cannot be empty")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "City must be between 1 and 100 characters")]
        public string City { get; set; }

        [Required(ErrorMessage = "Color is required and cannot be empty")]
        public string Color { get; set; }
    }
}