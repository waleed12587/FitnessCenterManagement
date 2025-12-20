using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Web.Models
{
    public class GymService
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Service Name")]
        public string Name { get; set; } = null!;

        [Required]
        [Range(10, 300, ErrorMessage = "Duration must be between 10 and 300 minutes.")]
        [Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "Price must be between 0 and 10000.")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Gym")]
        public int GymId { get; set; }

        public Gym? Gym { get; set; }
        public ICollection<TrainerService>? TrainerServices { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }

    }
}
