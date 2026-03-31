using System;
using System.Collections.Generic;

namespace TurAgenstvo.Model;

public partial class Employee
{
    public Guid EmployeeId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public int? PositionId { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public DateOnly? HireDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Financialoperation> FinancialOperations { get; set; } = new List<Financialoperation>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Position? Position { get; set; }
}
