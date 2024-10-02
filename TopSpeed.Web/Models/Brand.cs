using System.ComponentModel.DataAnnotations;

namespace TopSpeed.Web.Models
{
    public class Brand
    {
        [Key]
        public  Guid Id { get; set; }

        [Required(ErrorMessage = "Please Specify Name")]
        public string Name { get; set; }

        [Display(Name = "Established Year")]
        public int EstablishedYear { get; set; }

        [Display(Name = "Brand Logo")]

        public string BrandLogo {get; set; }
    }
}
