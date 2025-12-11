using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Web.Models
{
    public class GymService
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Hizmet Adı")]
        public string Name { get; set; } = null!;

        [Required]
        [Range(10, 300, ErrorMessage = "Süre 10 ile 300 dakika arasında olmalıdır.")]
        [Display(Name = "Süre (dakika)")]
        public int DurationMinutes { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "Ücret 0 ile 10000 arasında olmalıdır.")]
        [Display(Name = "Ücret (₺)")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Salon")]
        public int GymId { get; set; }

        public Gym? Gym { get; set; }
        public ICollection<TrainerService>? TrainerServices { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }

    }
}
