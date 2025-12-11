namespace FitnessCenter.Web.Models
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public int? HeightCm { get; set; }
        public float? WeightKg { get; set; }
        public string? BodyType { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
