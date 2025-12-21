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
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [Display(Name = "Time")]
        [DataType(DataType.Time)]
        public TimeSpan AppointmentTime { get; set; }

        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = "Cash";

        // Helper property to combine date and time
        public DateTime StartDateTime => AppointmentDate.Date.Add(AppointmentTime);
    }
}
