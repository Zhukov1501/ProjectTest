using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using Dapper;
using TravelAgencyApp.Models;

namespace TravelAgencyApp.Data;

public class DatabaseService
{
    private readonly string _connectionString;
    private static DatabaseService? _instance;
    private static readonly object _lock = new();

    public static DatabaseService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new DatabaseService();
                }
            }
            return _instance;
        }
    }

    private DatabaseService()
    {
        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TravelAgency.db");
        _connectionString = $"Data Source={dbPath}";
        InitializeDatabase();
    }

    public SqliteConnection GetConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    private void InitializeDatabase()
    {
        using var connection = GetConnection();
        connection.Open();

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                PasswordHash TEXT NOT NULL,
                FullName TEXT NOT NULL,
                Role TEXT DEFAULT 'User',
                CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
                IsActive INTEGER DEFAULT 1
            );

            CREATE TABLE IF NOT EXISTS Clients (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FirstName TEXT NOT NULL,
                LastName TEXT NOT NULL,
                MiddleName TEXT,
                PassportNumber TEXT,
                Phone TEXT,
                Email TEXT,
                BirthDate TEXT,
                Address TEXT,
                RegistrationDate TEXT DEFAULT CURRENT_TIMESTAMP,
                Notes TEXT,
                IsActive INTEGER DEFAULT 1
            );

            CREATE TABLE IF NOT EXISTS AttractionServices (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT,
                Category TEXT,
                Price REAL NOT NULL,
                DurationMinutes INTEGER,
                MaxParticipants INTEGER,
                AvailableSlots INTEGER,
                Location TEXT,
                AgeRestriction TEXT DEFAULT '0+',
                IsAvailable INTEGER DEFAULT 1,
                CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP
            );

            CREATE TABLE IF NOT EXISTS Bookings (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ClientId INTEGER NOT NULL,
                ServiceId INTEGER NOT NULL,
                BookingDate TEXT DEFAULT CURRENT_TIMESTAMP,
                VisitDate TEXT NOT NULL,
                ParticipantsCount INTEGER DEFAULT 1,
                TotalPrice REAL,
                Status TEXT DEFAULT 'Pending',
                Notes TEXT,
                CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (ClientId) REFERENCES Clients(Id),
                FOREIGN KEY (ServiceId) REFERENCES AttractionServices(Id)
            );
        ");

        SeedDefaultData(connection);
    }

    private void SeedDefaultData(SqliteConnection connection)
    {
        var userExists = connection.QueryFirstOrDefault<int>(
            "SELECT COUNT(*) FROM Users WHERE Username = @Username", 
            new { Username = "admin" });

        if (userExists == 0)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
            connection.Execute(@"
                INSERT INTO Users (Username, PasswordHash, FullName, Role)
                VALUES ('admin', @PasswordHash, 'Администратор', 'Admin'),
                       ('manager', @ManagerHash, 'Менеджер', 'Manager'),
                       ('user', @UserHash, 'Пользователь', 'User')",
                new { 
                    PasswordHash = passwordHash,
                    ManagerHash = BCrypt.Net.BCrypt.HashPassword("manager123"),
                    UserHash = BCrypt.Net.BCrypt.HashPassword("user123")
                });
        }

        var serviceExists = connection.QueryFirstOrDefault<int>(
            "SELECT COUNT(*) FROM AttractionServices");
        
        if (serviceExists == 0)
        {
            connection.Execute(@"
                INSERT INTO AttractionServices (Name, Description, Category, Price, DurationMinutes, MaxParticipants, AvailableSlots, Location, AgeRestriction, IsAvailable)
                VALUES 
                    ('Колесо обозрения', 'Панорамный вид на город с высоты 65 метров', 'Развлечения', 500, 30, 8, 48, 'Центральная площадь', '6+', 1),
                    ('Американские горки', 'Захватывающий аттракцион с резкими спусками', 'Экстрим', 800, 3, 24, 240, 'Зона A', '12+', 1),
                    ('Карусель парковая', 'Классическая карусель для всей семьи', 'Семейные', 300, 10, 40, 200, 'Зона B', '3+', 1),
                    ('Виртуальная реальность', 'Иммерсивные VR-приключения', 'Технологии', 1200, 45, 6, 30, 'Зона C', '8+', 1),
                    ('Аквапарк', 'Водные горки и бассейны', 'Водные', 1500, 180, 100, 500, 'Водная зона', '5+', 1),
                    ('Парк аттракционов', 'Комплекс из 15 аттракционов', 'Комплекс', 2500, 360, 200, 1000, 'Весь парк', '0+', 1),
                    ('5D Кинотеатр', 'Иммерсивный фильм с эффектами', 'Кино', 400, 20, 50, 200, 'Зона D', '5+', 1),
                    ('Лабиринт зеркальный', 'Забавный лабиринт из зеркал', 'Развлечения', 250, 15, 20, 100, 'Зона B', '3+', 1),
                    ('Прыжки на батуте', 'Профессиональные батуты', 'Спорт', 600, 30, 15, 75, 'Спортивная зона', '6+', 1),
                    ('Автодром', 'Электрические машинки для детей', 'Детские', 350, 15, 20, 120, 'Детская зона', '4+', 1)");            
        }

        var clientExists = connection.QueryFirstOrDefault<int>("SELECT COUNT(*) FROM Clients");
        if (clientExists == 0)
        {
            connection.Execute(@"
                INSERT INTO Clients (FirstName, LastName, MiddleName, PassportNumber, Phone, Email, BirthDate, Address, Notes) VALUES 
                ('Иван', 'Петров', 'Сергеевич', '1234 567890', '+7 999 123-45-67', 'ivan.petrov@mail.ru', '1990-05-15', 'г. Москва, ул. Ленина, д. 10', 'Постоянный клиент'),
                ('Мария', 'Сидорова', 'Александровна', '2345 678901', '+7 999 234-56-78', 'maria.sidorova@mail.ru', '1985-03-22', 'г. Москва, ул. Пушкина, д. 5', 'VIP клиент'),
                ('Алексей', 'Козлов', 'Дмитриевич', '3456 789012', '+7 999 345-67-89', 'alexey.kozlov@mail.ru', '1995-08-10', 'г. Москва, пр. Мира, д. 15', ''),
                ('Елена', 'Новикова', 'Игоревна', '4567 890123', '+7 999 456-78-90', 'elena.novikova@mail.ru', '1988-12-01', 'г. Москва, ул. Гагарина, д. 20', 'Предпочитает водные развлечения'),
                ('Дмитрий', 'Морозов', 'Владимирович', '5678 901234', '+7 999 567-89-01', 'dmitry.morozov@mail.ru', '1992-07-25', 'г. Москва, ул. Чехова, д. 8', ''),
                ('Ольга', 'Волкова', 'Петровна', '6789 012345', '+7 999 678-90-12', 'olga.volkova@mail.ru', '1987-11-30', 'г. Москва, ул. Толстого, д. 12', ''),
                ('Сергей', 'Лебедев', 'Андреевич', '7890 123456', '+7 999 789-01-23', 'sergey.lebedev@mail.ru', '1993-04-18', 'г. Москва, ул. Некрасова, д. 7', 'Любит экстрим'),
                ('Анна', 'Кузнецова', 'Михайловна', '8901 234567', '+7 999 890-12-34', 'anna.kuznetsova@mail.ru', '1991-09-05', 'г. Москва, ул. Гоголя, д. 3', ''),
                ('Николай', 'Соловьев', 'Викторович', '9012 345678', '+7 999 901-23-45', 'nikolay.solovyev@mail.ru', '1989-02-14', 'г. Москва, ул. Островского, д. 9', ''),
                ('Татьяна', 'Васильева', 'Николаевна', '1111 222233', '+7 999 111-22-33', 'tatiana.vasilieva@mail.ru', '1994-06-22', 'г. Москва, ул. Тургенева, д. 14', ''),
                ('Андрей', 'Федоров', 'Иванович', '2222 333344', '+7 999 222-33-44', 'andrey.fedorov@mail.ru', '1986-10-08', 'г. Москва, ул. Бунина, д. 6', ''),
                ('Наталья', 'Зайцева', 'Сергеевна', '3333 444455', '+7 999 333-44-55', 'natalia.zaitseva@mail.ru', '1996-01-17', 'г. Москва, ул. Достоевского, д. 11', ''),
                ('Владимир', 'Смирнов', 'Петрович', '4444 555566', '+7 999 444-55-66', 'vladimir.smirnov@mail.ru', '1984-08-25', 'г. Москва, ул. Платонова, д. 4', ''),
                ('Екатерина', 'Орлова', 'Алексеевна', '5555 666677', '+7 999 555-66-77', 'ekaterina.orlova@mail.ru', '1997-03-12', 'г. Москва, ул. Булгакова, д. 18', ''),
                ('Павел', 'Ильин', 'Дмитриевич', '6666 777788', '+7 999 666-77-88', 'pavel.ilin@mail.ru', '1983-12-03', 'г. Москва, ул. Есенина, д. 2', ''),
                ('Виктория', 'Григорьева', 'Владимировна', '7777 888899', '+7 999 777-88-99', 'viktoria.grigorieva@mail.ru', '1998-07-19', 'г. Москва, ул. Ахматовой, д. 16', ''),
                ('Артем', 'Романов', 'Олегович', '8888 999900', '+7 999 888-99-00', 'artem.romanov@mail.ru', '1992-11-28', 'г. Москва, ул. Цветаевой, д. 13', ''),
                ('Марина', 'Степанова', 'Григорьевна', '9999 000011', '+7 999 999-00-11', 'marina.stepanova@mail.ru', '1981-05-09', 'г. Москва, ул. Маяковского, д. 1', ''),
                ('Игорь', 'Николаев', 'Васильевич', '1010 202030', '+7 999 101-02-03', 'igor.nikolaev@mail.ru', '1990-09-14', 'г. Москва, ул. Пастернака, д. 19', ''),
                ('Светлана', 'Миронова', 'Константиновна', '2020 303040', '+7 999 202-03-04', 'svetlana.mironova@mail.ru', '1994-02-27', 'г. Москва, ул. Вознесенского, д. 21', ''),
                ('Константин', 'Павлов', 'Евгеньевич', '3030 405060', '+7 999 303-04-05', 'konstantin.pavlov@mail.ru', '1988-06-11', 'г. Москва, ул. Маршака, д. 8', ''),
                ('Людмила', 'Новикова', 'Федоровна', '4040 506070', '+7 999 404-05-06', 'liudmila.novikova@mail.ru', '1995-10-23', 'г. Москва, ул. Барто, д. 17', ''),
                ('Роман', 'Максимов', 'Сергеевич', '5050 607080', '+7 999 505-06-07', 'roman.maximov@mail.ru', '1989-01-06', 'г. Москва, ул. Михалкова, д. 22', ''),
                ('Оксана', 'Соколова', 'Павловна', '6060 708090', '+7 999 606-07-08', 'oksana.sokolova@mail.ru', '1993-08-31', 'г. Москва, ул. Пришвина, д. 25', ''),
                ('Денис', 'Воронов', 'Александрович', '7070 809010', '+7 999 707-08-09', 'denis.voronov@mail.ru', '1996-04-15', 'г. Москва, ул. Шолохова, д. 24', ''),
                ('Галина', 'Морозова', 'Ивановна', '8080 910111', '+7 999 808-09-10', 'galina.morozova@mail.ru', '1982-07-07', 'г. Москва, ул. Драгунского, д. 26', ''),
                ('Юрий', 'Алексеев', 'Николаевич', '9090 111213', '+7 999 909-11-12', 'yuri.alekseev@mail.ru', '1991-11-20', 'г. Москва, ул. Распутина, д. 27', ''),
                ('Валентина', 'Тимофеева', 'Михайловна', '1011 121314', '+7 999 011-12-13', 'valentina.timofeeva@mail.ru', '1986-03-04', 'г. Москва, ул. Снегова, д. 28', ''),
                ('Георгий', 'Чернов', 'Петрович', '1112 131415', '+7 999 112-13-14', 'georgiy.chernov@mail.ru', '1997-09-16', 'г. Москва, ул. Волкова, д. 29', '')");
        }
    }

    public User? ValidateUser(string username, string password)
    {
        using var connection = GetConnection();
        var user = connection.QueryFirstOrDefault<User>(
            "SELECT * FROM Users WHERE Username = @Username AND IsActive = 1",
            new { Username = username });

        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return user;
        }
        return null;
    }

    public IEnumerable<Client> GetClients(string? search = null, string? sortBy = null, bool ascending = true)
    {
        using var connection = GetConnection();
        var sql = "SELECT * FROM Clients WHERE IsActive = 1";
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            sql += @" AND (FirstName LIKE @Search OR LastName LIKE @Search 
                      OR MiddleName LIKE @Search OR Phone LIKE @Search 
                      OR Email LIKE @Search OR PassportNumber LIKE @Search)";
        }

        sql += sortBy switch
        {
            "Фамилия" => " ORDER BY LastName",
            "Имя" => " ORDER BY FirstName",
            "Дата рождения" => " ORDER BY BirthDate",
            "Дата регистрации" => " ORDER BY RegistrationDate",
            "Телефон" => " ORDER BY Phone",
            _ => " ORDER BY Id"
        };

        sql += ascending ? " ASC" : " DESC";

        return connection.Query<Client>(sql, new { Search = $"%{search}%" });
    }

    public Client? GetClientById(int id)
    {
        using var connection = GetConnection();
        return connection.QueryFirstOrDefault<Client>(
            "SELECT * FROM Clients WHERE Id = @Id", new { Id = id });
    }

    public int AddClient(Client client)
    {
        using var connection = GetConnection();
        return connection.ExecuteScalar<int>(@"
            INSERT INTO Clients (FirstName, LastName, MiddleName, PassportNumber, Phone, Email, BirthDate, Address, Notes)
            VALUES (@FirstName, @LastName, @MiddleName, @PassportNumber, @Phone, @Email, @BirthDate, @Address, @Notes);
            SELECT last_insert_rowid();", client);
    }

    public void UpdateClient(Client client)
    {
        using var connection = GetConnection();
        connection.Execute(@"
            UPDATE Clients SET 
                FirstName = @FirstName, LastName = @LastName, MiddleName = @MiddleName,
                PassportNumber = @PassportNumber, Phone = @Phone, Email = @Email,
                BirthDate = @BirthDate, Address = @Address, Notes = @Notes
            WHERE Id = @Id", client);
    }

    public void DeleteClient(int id)
    {
        using var connection = GetConnection();
        connection.Execute("UPDATE Clients SET IsActive = 0 WHERE Id = @Id", new { Id = id });
    }

    public IEnumerable<AttractionService> GetServices(string? search = null, string? category = null, 
        string? sortBy = null, bool ascending = true, decimal? minPrice = null, decimal? maxPrice = null)
    {
        using var connection = GetConnection();
        var sql = "SELECT * FROM AttractionServices WHERE IsAvailable = 1";
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            sql += @" AND (Name LIKE @Search OR Description LIKE @Search OR Location LIKE @Search)";
            parameters.Add("Search", $"%{search}%");
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            sql += " AND Category = @Category";
            parameters.Add("Category", category);
        }

        if (minPrice.HasValue)
        {
            sql += " AND Price >= @MinPrice";
            parameters.Add("MinPrice", minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            sql += " AND Price <= @MaxPrice";
            parameters.Add("MaxPrice", maxPrice.Value);
        }

        sql += sortBy switch
        {
            "Название" => " ORDER BY Name",
            "Цена" => " ORDER BY Price",
            "Длительность" => " ORDER BY DurationMinutes",
            "Категория" => " ORDER BY Category",
            _ => " ORDER BY Id"
        };

        sql += ascending ? " ASC" : " DESC";

        return connection.Query<AttractionService>(sql, parameters);
    }

    public IEnumerable<string> GetCategories()
    {
        using var connection = GetConnection();
        return connection.Query<string>("SELECT DISTINCT Category FROM AttractionServices WHERE IsAvailable = 1 ORDER BY Category");
    }

    public AttractionService? GetServiceById(int id)
    {
        using var connection = GetConnection();
        return connection.QueryFirstOrDefault<AttractionService>(
            "SELECT * FROM AttractionServices WHERE Id = @Id", new { Id = id });
    }

    public int AddService(AttractionService service)
    {
        using var connection = GetConnection();
        return connection.ExecuteScalar<int>(@"
            INSERT INTO AttractionServices (Name, Description, Category, Price, DurationMinutes, MaxParticipants, AvailableSlots, Location, AgeRestriction, IsAvailable)
            VALUES (@Name, @Description, @Category, @Price, @DurationMinutes, @MaxParticipants, @AvailableSlots, @Location, @AgeRestriction, @IsAvailable);
            SELECT last_insert_rowid();", service);
    }

    public void UpdateService(AttractionService service)
    {
        using var connection = GetConnection();
        connection.Execute(@"
            UPDATE AttractionServices SET 
                Name = @Name, Description = @Description, Category = @Category,
                Price = @Price, DurationMinutes = @DurationMinutes, MaxParticipants = @MaxParticipants,
                AvailableSlots = @AvailableSlots, Location = @Location, AgeRestriction = @AgeRestriction,
                IsAvailable = @IsAvailable
            WHERE Id = @Id", service);
    }

    public void DeleteService(int id)
    {
        using var connection = GetConnection();
        connection.Execute("UPDATE AttractionServices SET IsAvailable = 0 WHERE Id = @Id", new { Id = id });
    }

    public IEnumerable<Booking> GetBookings(int? clientId = null)
    {
        using var connection = GetConnection();
        var sql = @"
            SELECT b.*, c.FirstName, c.LastName, c.MiddleName, 
                   s.Name as ServiceName
            FROM Bookings b
            LEFT JOIN Clients c ON b.ClientId = c.Id
            LEFT JOIN AttractionServices s ON b.ServiceId = s.Id
            WHERE 1=1";

        if (clientId.HasValue)
            sql += " AND b.ClientId = @ClientId";

        return connection.Query<Booking>(sql, new { ClientId = clientId });
    }
}
