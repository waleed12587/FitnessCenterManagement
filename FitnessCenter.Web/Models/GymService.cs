namespace FitnessCenter.Web.Models
{
    public class GymService
    {
        public int Id { get; set; }
        public int GymId { get; set; }
        public string Name { get; set; } = null!;
        public int DurationMinutes { get; set; }
        public decimal Price { get; set; }

        public Gym Gym { get; set; } = null!;
        public ICollection<TrainerService> TrainerServices { get; set; } = new List<TrainerService>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
