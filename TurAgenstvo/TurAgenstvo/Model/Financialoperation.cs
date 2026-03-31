using System;
using System.Collections.Generic;

namespace TurAgenstvo.Model;

public partial class Financialoperation
{
    public Guid OperationId { get; set; }

    public string OperationNumber { get; set; } = null!;

    public Guid? BookingId { get; set; }

    public Guid? PaymentId { get; set; }

    public DateTime? OperationDate { get; set; }

    public string? OperationType { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public Guid? EmployeeId { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Payment? Payment { get; set; }
}
