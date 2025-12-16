using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Web.ViewModels
{
    public class AIWorkoutRequestVM
    {
        [Required(ErrorMessage = "Yaş gereklidir")]
        [Range(13, 100, ErrorMessage = "Yaş 13-100 arasında olmalıdır")]
        [Display(Name = "Yaş")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Boy gereklidir")]
        [Range(100, 250, ErrorMessage = "Boy 100-250 cm arasında olmalıdır")]
        [Display(Name = "Boy (cm)")]
        public int Height { get; set; }

        [Required(ErrorMessage = "Kilo gereklidir")]
        [Range(30, 200, ErrorMessage = "Kilo 30-200 kg arasında olmalıdır")]
        [Display(Name = "Kilo (kg)")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Cinsiyet seçilmelidir")]
        [Display(Name = "Cinsiyet")]
        public string Gender { get; set; } = "Erkek";

        [Required(ErrorMessage = "Hedef seçilmelidir")]
        [Display(Name = "Hedef")]
        public string Goal { get; set; } = "Kilo Verme";

        [Required(ErrorMessage = "Aktivite seviyesi seçilmelidir")]
        [Display(Name = "Aktivite Seviyesi")]
        public string ActivityLevel { get; set; } = "Orta";

        [Display(Name = "Sağlık Durumu / Notlar (Opsiyonel)")]
        [StringLength(500)]
        public string? HealthConditions { get; set; }
    }

    public class AIWorkoutResultVM
    {
        public AIWorkoutRequestVM Request { get; set; } = null!;
        public string Recommendation { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
    }
}

