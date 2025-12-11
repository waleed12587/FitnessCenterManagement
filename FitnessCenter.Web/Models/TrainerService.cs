namespace FitnessCenter.Web.Models
{
    public class TrainerService
    {
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; } = null!;

        public int GymServiceId { get; set; }
        public GymService GymService { get; set; } = null!;
    }

}
