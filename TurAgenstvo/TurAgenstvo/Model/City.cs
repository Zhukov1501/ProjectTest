using System;
using System.Collections.Generic;

namespace TurAgenstvo.Model;

public partial class City
{
    public int CityId { get; set; }

    public string CityName { get; set; } = null!;

    public int? CountryId { get; set; }

    public virtual Country? Country { get; set; }

    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();

    public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
