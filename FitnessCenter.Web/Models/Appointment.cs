namespace FitnessCenter.Web.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public string MemberId { get; set; } = null!; // Identity user
        public int TrainerId { get; set; }
        public int GymServiceId { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public decimal Price { get; set; }
        public AppointmentStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public DateTime? PaymentDate { get; set; }
        public string? PaymentMethod { get; set; }

        public ApplicationUser Member { get; set; } = null!;
        public Trainer Trainer { get; set; } = null!;
        public GymService GymService { get; set; } = null!;
    }

}
