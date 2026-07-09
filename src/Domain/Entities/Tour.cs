using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourManagement.Domain.Entities
{
    public class Tour
    {
        [Key]
        public int TourId { get; set; }
        [Required]
        public string TourName { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string Location { get; set; }
        public int DurationDays { get; set; }
        public string ImagePath { get; set; }
    }

    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        public string Role { get; set; } // Admin or User
        public string FullName { get; set; }
    }

    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int TourId { get; set; }
        public DateTime BookingDate { get; set; }
        public int NumberOfPeople { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } // Pending, Confirmed, Cancelled

        public virtual User User { get; set; }
        public virtual Tour Tour { get; set; }
    }
}
