using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Web.ViewModels
{
    public class AppointmentCreateVM
    {
        [Required]
        [Display(Name = "Hizmet")]
        public int GymServiceId { get; set; }

        [Required]
        [Display(Name = "Antrenör")]
        public int TrainerId { get; set; }

        [Required]
        [Display(Name = "Başlangıç Zamanı")]
        public DateTime StartDateTime { get; set; }
    }
}
