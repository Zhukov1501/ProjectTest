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

            CREATE TABLE IF NOT EXISTS Tours (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT,
                Country TEXT,
                City TEXT,
                TourType TEXT,
                Price REAL NOT NULL,
                DurationDays INTEGER,
                MaxParticipants INTEGER,
                AvailableSlots INTEGER,
                HotelName TEXT,
                HotelStars TEXT DEFAULT '3*',
                MealType TEXT DEFAULT 'Завтрак',
                IsAvailable INTEGER DEFAULT 1,
                CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP
            );

            CREATE TABLE IF NOT EXISTS Bookings (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ClientId INTEGER NOT NULL,
                TourId INTEGER NOT NULL,
                BookingDate TEXT DEFAULT CURRENT_TIMESTAMP,
                DepartureDate TEXT NOT NULL,
                ParticipantsCount INTEGER DEFAULT 1,
                TotalPrice REAL,
                Status TEXT DEFAULT 'Ожидание',
                Notes TEXT,
                CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (ClientId) REFERENCES Clients(Id),
                FOREIGN KEY (TourId) REFERENCES Tours(Id)
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
            connection.Execute(@"
                INSERT INTO Users (Username, PasswordHash, FullName, Role)
                VALUES ('admin', @AdminHash, 'Администратор', 'Admin'),
                       ('manager', @ManagerHash, 'Менеджер', 'Менеджер'),
                       ('user', @UserHash, 'Пользователь', 'Пользователь')",
                new { 
                    AdminHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    ManagerHash = BCrypt.Net.BCrypt.HashPassword("manager123"),
                    UserHash = BCrypt.Net.BCrypt.HashPassword("user123")
                });
        }

        var tourExists = connection.QueryFirstOrDefault<int>("SELECT COUNT(*) FROM Tours");
        if (tourExists == 0)
        {
            connection.Execute(@"
                INSERT INTO Tours (Name, Description, Country, City, TourType, Price, DurationDays, MaxParticipants, AvailableSlots, HotelName, HotelStars, MealType, IsAvailable) VALUES 
                ('Отдых на Мальдивах', 'Райский отдых на белоснежных пляжах, кристально чистое море, снорклинг', 'Мальдиды', 'Мале', 'Пляжный', 250000, 10, 4, 8, 'Baros Maldives', '5*', 'Всё включено', 1),
                ('Тур по Европе', 'Посещение Парижа, Рима и Барселоны с экскурсиями', 'Франция, Италия, Испания', 'Париж', 'Экскурсионный', 180000, 12, 20, 15, 'Hotel Europe', '4*', 'Завтрак', 1),
                ('Горнолыжный курорт Андорра', 'Катание на лыжах в Пиренеях, спа и ночная жизнь', 'Андорра', 'Андорра-ла-Велья', 'Горнолыжный', 95000, 7, 10, 20, 'Sport Hotel Hermitage', '5*', 'Полупансион', 1),
                ('Круиз по Средиземному морю', 'Путешествие на лайнере с остановками в Греции, Турции и Кипре', 'Греция', 'Афины', 'Круиз', 320000, 14, 6, 12, 'MSC Fantasia', '5*', 'Всё включено', 1),
                ('Таиланд - Пхукет', 'Пляжный отдых, слоны, храмы, ночные рынки', 'Таиланд', 'Пхукет', 'Пляжный', 120000, 14, 8, 25, 'The Pavilions Phuket', '5*', 'Завтрак', 1),
                ('Япония - Страна восходящего солнца', 'Токио, Киото, Хиросима, гора Фудзи', 'Япония', 'Токио', 'Экскурсионный', 280000, 10, 15, 10, 'Imperial Hotel Tokyo', '5*', 'Завтрак', 1),
                ('ОАЭ - Дубай', 'Шопинг, пляжи, пустыня, небоскрёбы', 'ОАЭ', 'Дубай', 'Пляжный', 150000, 7, 6, 30, 'Burj Al Arab', '5*', 'Завтрак', 1),
                ('Италия - Рим и Флоренция', 'Колизей, Ватикан, гастрономический тур', 'Италия', 'Рим', 'Экскурсионный', 140000, 8, 12, 18, 'Hotel de Russie', '5*', 'Завтрак', 1),
                ('Бали - Индонезия', 'Йога, храмы, рисовые террасы, вулканы', 'Индонезия', 'Денпасар', 'Оздоровительный', 110000, 12, 8, 15, 'Four Seasons Bali', '5*', 'Полупансион', 1),
                ('Сейшелы', 'Уединённый отдых на островах, дайвинг', 'Сейшелы', 'Виктория', 'Пляжный', 380000, 10, 4, 6, 'Four Seasons Seychelles', '5*', 'Всё включено', 1),
                ('Черногория - Будва', 'Адриатическое побережье, старый город, горы', 'Черногория', 'Будва', 'Пляжный', 75000, 10, 10, 22, 'Avala Resort', '4*', 'Завтрак', 1),
                ('Швейцария - Цюрих', 'Горные экскурсии, шоколад, часы', 'Швейцария', 'Цюрих', 'Горнолыжный', 200000, 7, 8, 12, 'The Dolder Grand', '5*', 'Завтрак', 1)");
        }

        var clientExists = connection.QueryFirstOrDefault<int>("SELECT COUNT(*) FROM Clients");
        if (clientExists == 0)
        {
            connection.Execute(@"
                INSERT INTO Clients (FirstName, LastName, MiddleName, PassportNumber, Phone, Email, BirthDate, Address, Notes) VALUES 
                ('Иван', 'Петров', 'Сергеевич', '1234 567890', '+7 999 123-45-67', 'ivan.petrov@mail.ru', '1990-05-15', 'г. Москва, ул. Ленина, д. 10', 'Постоянный клиент'),
                ('Мария', 'Сидорова', 'Александровна', '2345 678901', '+7 999 234-56-78', 'maria.sidorova@mail.ru', '1985-03-22', 'г. Москва, ул. Пушкина, д. 5', 'VIP клиент'),
                ('Алексей', 'Козлов', 'Дмитриевич', '3456 789012', '+7 999 345-67-89', 'alexey.kozlov@mail.ru', '1995-08-10', 'г. Москва, пр. Мира, д. 15', ''),
                ('Елена', 'Новикова', 'Игоревна', '4567 890123', '+7 999 456-78-90', 'elena.novikova@mail.ru', '1988-12-01', 'г. Москва, ул. Гагарина, д. 20', 'Предпочитает пляжный отдых'),
                ('Дмитрий', 'Морозов', 'Владимирович', '5678 901234', '+7 999 567-89-01', 'dmitry.morozov@mail.ru', '1992-07-25', 'г. Москва, ул. Чехова, д. 8', ''),
                ('Ольга', 'Волкова', 'Петровна', '6789 012345', '+7 999 678-90-12', 'olga.volkova@mail.ru', '1987-11-30', 'г. Москва, ул. Толстого, д. 12', ''),
                ('Сергей', 'Лебедев', 'Андреевич', '7890 123456', '+7 999 789-01-23', 'sergey.lebedev@mail.ru', '1993-04-18', 'г. Москва, ул. Некрасова, д. 7', 'Любит экскурсионные туры'),
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

    public IEnumerable<Tour> GetTours(string? search = null, string? tourType = null, 
        string? country = null, string? sortBy = null, bool ascending = true, 
        decimal? minPrice = null, decimal? maxPrice = null)
    {
        using var connection = GetConnection();
        var sql = "SELECT * FROM Tours WHERE IsAvailable = 1";
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            sql += @" AND (Name LIKE @Search OR Description LIKE @Search OR Country LIKE @Search OR City LIKE @Search OR HotelName LIKE @Search)";
            parameters.Add("Search", $"%{search}%");
        }

        if (!string.IsNullOrWhiteSpace(tourType))
        {
            sql += " AND TourType = @TourType";
            parameters.Add("TourType", tourType);
        }

        if (!string.IsNullOrWhiteSpace(country))
        {
            sql += " AND Country LIKE @Country";
            parameters.Add("Country", $"%{country}%");
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
            "Страна" => " ORDER BY Country",
            "Тип тура" => " ORDER BY TourType",
            "Длительность" => " ORDER BY DurationDays",
            _ => " ORDER BY Id"
        };

        sql += ascending ? " ASC" : " DESC";

        return connection.Query<Tour>(sql, parameters);
    }

    public IEnumerable<string> GetTourTypes()
    {
        using var connection = GetConnection();
        return connection.Query<string>("SELECT DISTINCT TourType FROM Tours WHERE IsAvailable = 1 ORDER BY TourType");
    }

    public IEnumerable<string> GetCountries()
    {
        using var connection = GetConnection();
        return connection.Query<string>("SELECT DISTINCT Country FROM Tours WHERE IsAvailable = 1 ORDER BY Country");
    }

    public Tour? GetTourById(int id)
    {
        using var connection = GetConnection();
        return connection.QueryFirstOrDefault<Tour>(
            "SELECT * FROM Tours WHERE Id = @Id", new { Id = id });
    }

    public int AddTour(Tour tour)
    {
        using var connection = GetConnection();
        return connection.ExecuteScalar<int>(@"
            INSERT INTO Tours (Name, Description, Country, City, TourType, Price, DurationDays, MaxParticipants, AvailableSlots, HotelName, HotelStars, MealType, IsAvailable)
            VALUES (@Name, @Description, @Country, @City, @TourType, @Price, @DurationDays, @MaxParticipants, @AvailableSlots, @HotelName, @HotelStars, @MealType, @IsAvailable);
            SELECT last_insert_rowid();", tour);
    }

    public void UpdateTour(Tour tour)
    {
        using var connection = GetConnection();
        connection.Execute(@"
            UPDATE Tours SET 
                Name = @Name, Description = @Description, Country = @Country, City = @City,
                TourType = @TourType, Price = @Price, DurationDays = @DurationDays,
                MaxParticipants = @MaxParticipants, AvailableSlots = @AvailableSlots,
                HotelName = @HotelName, HotelStars = @HotelStars, MealType = @MealType,
                IsAvailable = @IsAvailable
            WHERE Id = @Id", tour);
    }

    public void DeleteTour(int id)
    {
        using var connection = GetConnection();
        connection.Execute("UPDATE Tours SET IsAvailable = 0 WHERE Id = @Id", new { Id = id });
    }

    public IEnumerable<Booking> GetBookings(int? clientId = null)
    {
        using var connection = GetConnection();
        var sql = @"
            SELECT b.*, c.FirstName, c.LastName, c.MiddleName, 
                   t.Name as TourName
            FROM Bookings b
            LEFT JOIN Clients c ON b.ClientId = c.Id
            LEFT JOIN Tours t ON b.TourId = t.Id
            WHERE 1=1";

        if (clientId.HasValue)
            sql += " AND b.ClientId = @ClientId";

        return connection.Query<Booking>(sql, new { ClientId = clientId });
    }
}
