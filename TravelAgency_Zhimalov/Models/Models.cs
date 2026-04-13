using System;

namespace TravelAgencyApp.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsActive { get; set; } = true;
}

public class Client
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string PassportNumber { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public string Notes { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
}

public class Tour
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string TourType { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public int MaxParticipants { get; set; }
    public int AvailableSlots { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string HotelStars { get; set; } = "3*";
    public string MealType { get; set; } = "Завтрак";
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class Booking
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int TourId { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime DepartureDate { get; set; }
    public int ParticipantsCount { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Ожидание";
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Client? Client { get; set; }
    public Tour? Tour { get; set; }
}
