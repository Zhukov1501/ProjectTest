using System;
using System.Collections.Generic;

namespace TurAgenstvo.Model;

public partial class Bookingtourist
{
    public int BookingTouristId { get; set; }

    public Guid? BookingId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? PassportSeries { get; set; }

    public string? PassportNumber { get; set; }

    public bool? IsMainTourist { get; set; }

    public virtual Booking? Booking { get; set; }
}
