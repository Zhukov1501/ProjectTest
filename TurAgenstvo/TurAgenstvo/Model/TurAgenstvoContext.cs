using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TurAgenstvo.Model;

public partial class TurAgenstvoContext : DbContext
{
    public TurAgenstvoContext()
    {
    }

    public TurAgenstvoContext(DbContextOptions<TurAgenstvoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Bookingtourist> Bookingtourists { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Financialoperation> FinancialOperations { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<Mealtype> MealTypes { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Tour> Tours { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=TurAgenstvo;Username=postgres;Password=admin123");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("bookings_pkey");

            entity.ToTable("bookings");

            entity.HasIndex(e => e.BookingNumber, "bookings_bookingnumber_key").IsUnique();

            entity.Property(e => e.BookingId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("bookingid");
            entity.Property(e => e.AdultsCount)
                .HasDefaultValue(1)
                .HasColumnName("adultscount");
            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("bookingdate");
            entity.Property(e => e.BookingNumber)
                .HasMaxLength(20)
                .HasColumnName("bookingnumber");
            entity.Property(e => e.ChildrenCount)
                .HasDefaultValue(0)
                .HasColumnName("childrencount");
            entity.Property(e => e.ClientId).HasColumnName("clientid");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeid");
            entity.Property(e => e.EndDate).HasColumnName("enddate");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.StartDate).HasColumnName("startdate");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Подтверждено'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(10, 2)
                .HasColumnName("totalprice");
            entity.Property(e => e.TourId).HasColumnName("tourid");

            entity.HasOne(d => d.Client).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("bookings_clientid_fkey");

            entity.HasOne(d => d.Employee).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("bookings_employeeid_fkey");

            entity.HasOne(d => d.Tour).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.TourId)
                .HasConstraintName("bookings_tourid_fkey");
        });

        modelBuilder.Entity<Bookingtourist>(entity =>
        {
            entity.HasKey(e => e.BookingTouristId).HasName("bookingtourists_pkey");

            entity.ToTable("bookingtourists");

            entity.Property(e => e.BookingTouristId).HasColumnName("bookingtouristid");
            entity.Property(e => e.BirthDate).HasColumnName("birthdate");
            entity.Property(e => e.BookingId).HasColumnName("bookingid");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("firstname");
            entity.Property(e => e.IsMainTourist)
                .HasDefaultValue(false)
                .HasColumnName("ismaintourist");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("lastname");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(100)
                .HasColumnName("middlename");
            entity.Property(e => e.PassportNumber)
                .HasMaxLength(6)
                .HasColumnName("passportnumber");
            entity.Property(e => e.PassportSeries)
                .HasMaxLength(4)
                .HasColumnName("passportseries");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingTourists)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("bookingtourists_bookingid_fkey");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("cities_pkey");

            entity.ToTable("cities");

            entity.Property(e => e.CityId).HasColumnName("cityid");
            entity.Property(e => e.CityName)
                .HasMaxLength(100)
                .HasColumnName("cityname");
            entity.Property(e => e.CountryId).HasColumnName("countryid");

            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("cities_countryid_fkey");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("clients_pkey");

            entity.ToTable("clients");

            entity.Property(e => e.ClientId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("clientid");
            entity.Property(e => e.BirthDate).HasColumnName("birthdate");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("firstname");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("lastname");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(100)
                .HasColumnName("middlename");
            entity.Property(e => e.PassportIssueDate).HasColumnName("passportissuedate");
            entity.Property(e => e.PassportIssuedby).HasColumnName("passportissuedby");
            entity.Property(e => e.PassportNumber)
                .HasMaxLength(6)
                .HasColumnName("passportnumber");
            entity.Property(e => e.PassportSeries)
                .HasMaxLength(4)
                .HasColumnName("passportseries");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("registrationdate");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("countries_pkey");

            entity.ToTable("countries");

            entity.HasIndex(e => e.CountryName, "countries_countryname_key").IsUnique();

            entity.Property(e => e.CountryId).HasColumnName("countryid");
            entity.Property(e => e.CountryName)
                .HasMaxLength(100)
                .HasColumnName("countryname");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("employees_pkey");

            entity.ToTable("employees");

            entity.HasIndex(e => e.Login, "employees_login_key").IsUnique();

            entity.Property(e => e.EmployeeId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("employeeid");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("firstname");
            entity.Property(e => e.HireDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("hiredate");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("lastname");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(100)
                .HasColumnName("middlename");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.PositionId).HasColumnName("positionid");

            entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("employees_positionid_fkey");
        });

        modelBuilder.Entity<Financialoperation>(entity =>
        {
            entity.HasKey(e => e.OperationId).HasName("financialoperations_pkey");

            entity.ToTable("financialoperations");

            entity.HasIndex(e => e.OperationNumber, "financialoperations_operationnumber_key").IsUnique();

            entity.Property(e => e.OperationId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("operationid");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.BookingId).HasColumnName("bookingid");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeid");
            entity.Property(e => e.OperationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("operationdate");
            entity.Property(e => e.OperationNumber)
                .HasMaxLength(20)
                .HasColumnName("operationnumber");
            entity.Property(e => e.OperationType)
                .HasMaxLength(50)
                .HasColumnName("operationtype");
            entity.Property(e => e.PaymentId).HasColumnName("paymentid");

            entity.HasOne(d => d.Booking).WithMany(p => p.FinancialOperations)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("financialoperations_bookingid_fkey");

            entity.HasOne(d => d.Employee).WithMany(p => p.FinancialOperations)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("financialoperations_employeeid_fkey");

            entity.HasOne(d => d.Payment).WithMany(p => p.FinancialOperations)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("financialoperations_paymentid_fkey");
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.HotelId).HasName("hotels_pkey");

            entity.ToTable("hotels");

            entity.Property(e => e.HotelId).HasColumnName("hotelid");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.CityId).HasColumnName("cityid");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.HotelName)
                .HasMaxLength(200)
                .HasColumnName("hotelname");
            entity.Property(e => e.MealtypeId).HasColumnName("mealtypeid");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Stars).HasColumnName("stars");

            entity.HasOne(d => d.City).WithMany(p => p.Hotels)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("hotels_cityid_fkey");

            entity.HasOne(d => d.MealType).WithMany(p => p.Hotels)
                .HasForeignKey(d => d.MealtypeId)
                .HasConstraintName("hotels_mealtypeid_fkey");
        });

        modelBuilder.Entity<Mealtype>(entity =>
        {
            entity.HasKey(e => e.MealTypeId).HasName("mealtypes_pkey");

            entity.ToTable("mealtypes");

            entity.HasIndex(e => e.MealCode, "mealtypes_mealcode_key").IsUnique();

            entity.Property(e => e.MealTypeId).HasColumnName("mealtypeid");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.MealCode)
                .HasMaxLength(10)
                .HasColumnName("mealcode");
            entity.Property(e => e.MealName)
                .HasMaxLength(100)
                .HasColumnName("mealname");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("payments_pkey");

            entity.ToTable("payments");

            entity.Property(e => e.PaymentId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("paymentid");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.BookingId).HasColumnName("bookingid");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeid");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("paymentdate");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("paymentmethod");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Оплачено'::character varying")
                .HasColumnName("paymentstatus");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("payments_bookingid_fkey");

            entity.HasOne(d => d.Employee).WithMany(p => p.Payments)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("payments_employeeid_fkey");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("positions_pkey");

            entity.ToTable("positions");

            entity.HasIndex(e => e.PositionName, "positions_positionname_key").IsUnique();

            entity.Property(e => e.PositionId).HasColumnName("positionid");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.PositionName)
                .HasMaxLength(50)
                .HasColumnName("positionname");
        });

        modelBuilder.Entity<Tour>(entity =>
        {
            entity.HasKey(e => e.TourId).HasName("tours_pkey");

            entity.ToTable("tours");

            entity.Property(e => e.TourId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("tourid");
            entity.Property(e => e.CityId).HasColumnName("cityid");
            entity.Property(e => e.CountryId).HasColumnName("countryid");
            entity.Property(e => e.DepartureDate).HasColumnName("departuredate");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DurationDays)
                .HasComputedColumnSql("(returndate - departuredate)", true)
                .HasColumnName("durationdays");
            entity.Property(e => e.HotelId).HasColumnName("hotelid");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.MaxTourists)
                .HasDefaultValue(1)
                .HasColumnName("maxtourists");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.ReturnDate).HasColumnName("returndate");
            entity.Property(e => e.TourName)
                .HasMaxLength(200)
                .HasColumnName("tourname");

            entity.HasOne(d => d.City).WithMany(p => p.Tours)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("tours_cityid_fkey");

            entity.HasOne(d => d.Country).WithMany(p => p.Tours)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("tours_countryid_fkey");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Tours)
                .HasForeignKey(d => d.HotelId)
                .HasConstraintName("tours_hotelid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
