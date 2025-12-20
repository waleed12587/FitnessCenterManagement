using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Web.ViewModels
{
    public class AppointmentCreateVM
    {
        [Required]
        [Display(Name = "Service")]
        public int GymServiceId { get; set; }

        [Required]
        [Display(Name = "Trainer")]
        public int TrainerId { get; set; }

        [Required]
        [Display(Name = "Start Date & Time")]
        public DateTime StartDateTime { get; set; }
    }
}
