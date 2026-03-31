using System;
using System.Collections.Generic;

namespace TurAgenstvo.Model;

public partial class Payment
{
    public Guid PaymentId { get; set; }

    public Guid? BookingId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public Guid? EmployeeId { get; set; }

    public string? Notes { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<Financialoperation> FinancialOperations { get; set; } = new List<Financialoperation>();
}
