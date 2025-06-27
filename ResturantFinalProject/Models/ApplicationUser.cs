using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ResturantFinalProject.Models
{
    public enum StaffRole
    {
        None, // For regular customers
        Chef,
        Waiter,
        Manager
    }

    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public int? CartId { get; set; }

        [ForeignKey("CartId")]
        public Cart? Cart { get; set; }

        public ICollection<Order>? Orders { get; set; } // Customer orders

        public StaffRole Role { get; set; } = StaffRole.None; // Default to None for customers

        public ICollection<Order>? AssignedOrders { get; set; } // Orders assigned to staff
    }
}