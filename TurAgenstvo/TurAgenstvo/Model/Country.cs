using System;
using System.Collections.Generic;

namespace TurAgenstvo.Model;

public partial class Country
{
    public int CountryId { get; set; }

    public string CountryName { get; set; } = null!;

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
