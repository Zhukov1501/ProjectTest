using System;
using System.Collections.Generic;

namespace TurAgenstvo.Model;

public partial class Hotel
{
    public int HotelId { get; set; }

    public string HotelName { get; set; } = null!;

    public int? CityId { get; set; }

    public int? Stars { get; set; }

    public int? MealtypeId { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Description { get; set; }

    public virtual City? City { get; set; }

    public virtual Mealtype? MealType { get; set; }

    public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
