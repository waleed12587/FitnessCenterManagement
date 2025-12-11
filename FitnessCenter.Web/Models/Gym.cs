namespace FitnessCenter.Web.Models
{
    public class Gym
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }

        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }

        public ICollection<GymService> Services { get; set; } = new List<GymService>();
        public ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
    }

}
