namespace TourManagement.Domain.Entities
{
    public class Tour
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public string Destination { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // Admin or User
    }

    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TourId { get; set; }
        public DateTime BookingDate { get; set; }
        public int NumberOfPeople { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending";

        public User? User { get; set; }
        public Tour? Tour { get; set; }
    }
}
