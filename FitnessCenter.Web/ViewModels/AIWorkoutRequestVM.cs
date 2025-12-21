using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FitnessCenter.Web.ViewModels
{
    public class AIWorkoutRequestVM
    {
        [Required(ErrorMessage = "Age is required")]
        [Range(13, 100, ErrorMessage = "Age must be between 13-100")]
        [Display(Name = "Age")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Height is required")]
        [Range(100, 250, ErrorMessage = "Height must be between 100-250 cm")]
        [Display(Name = "Height (cm)")]
        public int Height { get; set; }

        [Required(ErrorMessage = "Weight is required")]
        [Range(30, 200, ErrorMessage = "Weight must be between 30-200 kg")]
        [Display(Name = "Weight (kg)")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Gender must be selected")]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = "Male";

        [Required(ErrorMessage = "Goal must be selected")]
        [Display(Name = "Goal")]
        public string Goal { get; set; } = "Weight Loss";

        [Required(ErrorMessage = "Activity level must be selected")]
        [Display(Name = "Activity Level")]
        public string ActivityLevel { get; set; } = "Moderate";

        [Display(Name = "Health Conditions / Notes (Optional)")]
        [StringLength(500)]
        public string? HealthConditions { get; set; }

        [Display(Name = "Upload Your Photo (Optional)")]
        public IFormFile? Photo { get; set; }
    }

    public class AIWorkoutResultVM
    {
        public AIWorkoutRequestVM Request { get; set; } = null!;
        public string Recommendation { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public string? TransformationImageUrl { get; set; }
        public string? OriginalPhotoUrl { get; set; }
    }
}


