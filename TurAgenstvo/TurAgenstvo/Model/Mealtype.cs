using System;
using System.Collections.Generic;

namespace TurAgenstvo.Model;

public partial class Mealtype
{
    public int MealTypeId { get; set; } 

    public string MealCode { get; set; } = null!;

    public string MealName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
}
