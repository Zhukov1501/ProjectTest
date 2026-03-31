using System;
using System.Collections.Generic;
using TurAgenstvo.Model;
using TurAgenstvo.Helpers;

namespace TurAgenstvo.Model;

public partial class Booking
{
    public Guid BookingId { get; set; }

    public string BookingNumber { get; set; } = null!;

    public Guid? TourId { get; set; }

    public Guid? EmployeeId { get; set; }

    public Guid? ClientId { get; set; }

    public DateTime? BookingDate { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int? AdultsCount { get; set; }

    public int? ChildrenCount { get; set; }

    public decimal TotalPrice { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<Bookingtourist> BookingTourists { get; set; } = new List<Bookingtourist>();

    public virtual Client? Client { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<Financialoperation> FinancialOperations { get; set; } = new List<Financialoperation>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Tour? Tour { get; set; }

}