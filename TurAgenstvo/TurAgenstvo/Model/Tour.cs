using System;
using System.Collections.Generic;

namespace TurAgenstvo.Model;

public partial class Tour
{
    public Guid TourId { get; set; }

    public string TourName { get; set; } = null!;

    public int? CountryId { get; set; }

    public int? CityId { get; set; }

    public int? HotelId { get; set; }

    public DateOnly DepartureDate { get; set; }

    public DateOnly ReturnDate { get; set; }

    public int? DurationDays { get; set; }

    public decimal Price { get; set; }

    public int? MaxTourists { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual City? City { get; set; }

    public virtual Country? Country { get; set; }

    public virtual Hotel? Hotel { get; set; }
}
