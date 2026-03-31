using System;
using System.Collections.Generic;

namespace TurAgenstvo.Model;

public partial class Client
{
    public Guid ClientId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public string? PassportSeries { get; set; }

    public string? PassportNumber { get; set; }

    public string? PassportIssuedby { get; set; }

    public DateOnly? PassportIssueDate { get; set; }

    public DateOnly? RegistrationDate { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
