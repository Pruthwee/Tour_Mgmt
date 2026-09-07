using System;
using System.Collections.Generic;

namespace TourManagement.Domain.Entities
{
    public class Tour
    {
        public int Id { get; set; }
        public string TourName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public string Destination { get; set; }
        public string ImagePath { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } // Admin or User
    }

    public class Booking
    {
        public int Id { get; set; }
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
