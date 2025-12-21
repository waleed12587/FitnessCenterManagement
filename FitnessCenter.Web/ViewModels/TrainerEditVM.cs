using System.ComponentModel.DataAnnotations;
using FitnessCenter.Web.Models;

namespace FitnessCenter.Web.ViewModels
{
    public class ServiceCheckboxVM
    {
        public int GymServiceId { get; set; }
        public string Name { get; set; } = "";
        public bool IsSelected { get; set; }
    }

    public class TrainerCreateVM
    {
        [Required]
        public string FullName { get; set; } = "";

        public string? Specialty { get; set; }
        public string? Bio { get; set; }

        public int? GymId { get; set; }

        // List of services with checkboxes
        public List<ServiceCheckboxVM> Services { get; set; } = new();
    }

    public class TrainerEditVM
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = "";

        public string? Specialty { get; set; }
        public string? Bio { get; set; }

        public int? GymId { get; set; }

        // قائمة الخدمات مع checkboxes
        public List<ServiceCheckboxVM> Services { get; set; } = new();
    }
}
