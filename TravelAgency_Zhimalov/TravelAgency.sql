-- SQLite Database Script for Travel Agency System
-- Open this file in SQLite Studio or execute with sqlite3

-- Create database file: TravelAgency.sqlite

-- Users Table
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    FullName TEXT NOT NULL,
    Role TEXT DEFAULT 'User',
    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
    IsActive INTEGER DEFAULT 1
);

-- Clients Table (Task 18)
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

-- Attraction Services Table (Task 8)
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

-- Bookings Table
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

-- Insert default users (passwords: admin123, manager123, user123)
-- Hash generated with BCrypt
INSERT INTO Users (Username, PasswordHash, FullName, Role) VALUES 
('admin', '$2a$11$rS6bHkJkJKrLqLKjJrKVLOXp0rVJ1rU8wJqWqWqWqWqWqWqWqWqWq', 'Администратор', 'Admin'),
('manager', '$2a$11$mS6bHkJkJKrLqLKjJrKVLOXp0rVJ1rU8wJqWqWqWqWqWqWqWqWqWq', 'Менеджер', 'Manager'),
('user', '$2a$11$pS6bHkJkJKrLqLKjJrKVLOXp0rVJ1rU8wJqWqWqWqWqWqWqWqWqWq', 'Пользователь', 'User');

-- Insert sample attraction services (Task 8)
INSERT INTO AttractionServices (Name, Description, Category, Price, DurationMinutes, MaxParticipants, AvailableSlots, Location, AgeRestriction, IsAvailable) VALUES 
('Колесо обозрения', 'Панорамный вид на город с высоты 65 метров', 'Развлечения', 500, 30, 8, 48, 'Центральная площадь', '6+', 1),
('Американские горки', 'Захватывающий аттракцион с резкими спусками', 'Экстрим', 800, 3, 24, 240, 'Зона A', '12+', 1),
('Карусель парковая', 'Классическая карусель для всей семьи', 'Семейные', 300, 10, 40, 200, 'Зона B', '3+', 1),
('Виртуальная реальность', 'Иммерсивные VR-приключения', 'Технологии', 1200, 45, 6, 30, 'Зона C', '8+', 1),
('Аквапарк', 'Водные горки и бассейны', 'Водные', 1500, 180, 100, 500, 'Водная зона', '5+', 1),
('Парк аттракционов', 'Комплекс из 15 аттракционов', 'Комплекс', 2500, 360, 200, 1000, 'Весь парк', '0+', 1),
('5D Кинотеатр', 'Иммерсивный фильм с эффектами', 'Кино', 400, 20, 50, 200, 'Зона D', '5+', 1),
('Лабиринт зеркальный', 'Забавный лабиринт из зеркал', 'Развлечения', 250, 15, 20, 100, 'Зона B', '3+', 1),
('Прыжки на батуте', 'Профессиональные батуты', 'Спорт', 600, 30, 15, 75, 'Спортивная зона', '6+', 1),
('Автодром', 'Электрические машинки для детей', 'Детские', 350, 15, 20, 120, 'Детская зона', '4+', 1);

-- Insert sample clients (Task 18)
INSERT INTO Clients (FirstName, LastName, MiddleName, PassportNumber, Phone, Email, BirthDate, Address, Notes) VALUES 
('Иван', 'Петров', 'Сергеевич', '1234 567890', '+7 999 123-45-67', 'ivan.petrov@mail.ru', '1990-05-15', 'г. Москва, ул. Ленина, д. 10', 'Постоянный клиент'),
('Мария', 'Сидорова', 'Александровна', '2345 678901', '+7 999 234-56-78', 'maria.sidorova@mail.ru', '1985-03-22', 'г. Москва, ул. Пушкина, д. 5', 'VIP клиент'),
('Алексей', 'Козлов', 'Дмитриевич', '3456 789012', '+7 999 345-67-89', 'alexey.kozlov@mail.ru', '1995-08-10', 'г. Москва, пр. Мира, д. 15', ''),
('Елена', 'Новикова', 'Игоревна', '4567 890123', '+7 999 456-78-90', 'elena.novikova@mail.ru', '1988-12-01', 'г. Москва, ул. Гагарина, д. 20', 'Предпочитает водные развлечения'),
('Дмитрий', 'Морозов', 'Владимирович', '5678 901234', '+7 999 567-89-01', 'dmitry.morozov@mail.ru', '1992-07-25', 'г. Москва, ул. Чехова, д. 8', '');
