using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using TurAgenstvo.Model;
using TurAgenstvo.Extensions;

namespace TurAgenstvo.Pages
{
    public partial class AddBookingWindow : Window
    {
        public AddBookingWindow()
        {
            InitializeComponent();
            LoadTours();
        }

        private void LoadTours()
        {
            try
            {
                var tours = App.DbContext.Tours
                    .Where(t => t.IsActive == true)
                    .Include(t => t.Hotel)
                    .ToList();

                var tourDisplayList = tours.Select(t => new
                {
                    t.TourId,
                    DisplayName = $"{t.TourName} - {t.Price:N0} ₽",
                    Tour = t
                }).ToList();

                TourComboBox.ItemsSource = tourDisplayList;
                TourComboBox.DisplayMemberPath = "DisplayName";
                TourComboBox.SelectedValuePath = "TourId";
                // #region agent log
                System.IO.File.AppendAllText(@"debug-378db9.log", "{\"sessionId\":\"378db9\",\"hypothesisId\":\"H-D\",\"location\":\"AddBookingWindow.xaml.cs:LoadTours\",\"message\":\"TourComboBox configured\",\"data\":{\"selectedValuePath\":\"" + TourComboBox.SelectedValuePath + "\",\"toursCount\":" + tourDisplayList.Count + "},\"timestamp\":" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "}\n");
                // #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки туров: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void TourComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (TourComboBox.SelectedItem != null)
            {
                var selected = TourComboBox.SelectedItem as dynamic;
                var tour = selected.Tour as Tour;

                if (tour != null)
                {
                    TourInfoText.Text = $"{tour.TourName}\n" +
                                       $"Отель: {tour.Hotel?.HotelName} ({tour.Hotel?.Stars}★)\n" +
                                       $"Вылет: {tour.DepartureDate:dd.MM.yyyy}\n" +
                                       $"Цена за человека: {tour.Price:N0} ₽";

                    StartDatePicker.SelectedDate = tour.DepartureDate.ToDateTime();
                    EndDatePicker.SelectedDate = tour.ReturnDate.ToDateTime();

                    CalculateTotalPrice();
                }
            }
        }

        private void CalculateTotalPrice()
        {
            if (TourComboBox.SelectedItem == null) return;

            var selected = TourComboBox.SelectedItem as dynamic;
            var tour = selected.Tour as Tour;

            if (tour != null)
            {
                int adults = int.TryParse(AdultsCountBox.Text, out int a) ? a : 0;
                int children = int.TryParse(ChildrenCountBox.Text, out int c) ? c : 0;

                decimal childDiscount = 0.5m;
                decimal totalPrice = (adults * tour.Price) + (children * tour.Price * childDiscount);

                TotalPriceText.Text = $"{totalPrice:N0} руб";
            }
        }

        private void AdultsCountBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CalculateTotalPrice();
        }

        private void ChildrenCountBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CalculateTotalPrice();
        }

        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            SaveBooking(false);
        }

        private void BookAndPayButton_Click(object sender, RoutedEventArgs e)
        {
            SaveBooking(true);
        }

        private void SaveBooking(bool markAsPaid)
        {
            if (TourComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(ClientNameBox.Text) ||
                string.IsNullOrWhiteSpace(ClientPhoneBox.Text) ||
                StartDatePicker.SelectedDate == null ||
                EndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Заполните все обязательные поля!",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return;
            }

            try
            {
                var client = FindOrCreateClient();

                var selected = TourComboBox.SelectedItem as dynamic;
                var tour = selected.Tour as Tour;

                if (!int.TryParse(AdultsCountBox.Text, out int adults) || adults < 1)
                {
                    MessageBox.Show("Укажите корректное количество взрослых (минимум 1).",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!int.TryParse(ChildrenCountBox.Text, out int children) || children < 0)
                    children = 0;
                decimal childDiscount = 0.5m;
                decimal totalPrice = (adults * tour.Price) + (children * tour.Price * childDiscount);

                var booking = new Booking
                {
                    BookingId = Guid.NewGuid(),
                    BookingNumber = $"BR-{DateTime.Now:yyyyMMdd-HHmmss}",
                    TourId = tour.TourId,
                    EmployeeId = MainWindow.CurrentEmployee?.EmployeeId,
                    ClientId = client.ClientId,
                    BookingDate = DateTime.Now,
                    StartDate = DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value),
                    EndDate = DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value),
                    AdultsCount = adults,
                    ChildrenCount = children,
                    TotalPrice = totalPrice,
                    Status = markAsPaid ? "Оплачено" : "Подтверждено"
                };

                App.DbContext.Bookings.Add(booking);

                if (markAsPaid)
                {
                    var payment = new Payment
                    {
                        PaymentId = Guid.NewGuid(),
                        BookingId = booking.BookingId,
                        PaymentDate = DateTime.Now,
                        Amount = totalPrice,
                        PaymentMethod = "Наличные",
                        PaymentStatus = "Оплачено",
                        EmployeeId = MainWindow.CurrentEmployee?.EmployeeId
                    };

                    App.DbContext.Payments.Add(payment);
                }

                App.DbContext.SaveChanges();

                MessageBox.Show($"Бронирование успешно создано!\nНомер брони: {booking.BookingNumber}",
                              "Успех",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении бронирования: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private Client FindOrCreateClient()
        {
            var client = App.DbContext.Clients
                .FirstOrDefault(c => c.Phone == ClientPhoneBox.Text.Trim());

            if (client == null)
            {
                var nameParts = ClientNameBox.Text.Trim().Split(' ');

                client = new Client
                {
                    ClientId = Guid.NewGuid(),
                    LastName = nameParts.Length > 0 ? nameParts[0] : "",
                    FirstName = nameParts.Length > 1 ? nameParts[1] : "",
                    MiddleName = nameParts.Length > 2 ? nameParts[2] : "",
                    Phone = ClientPhoneBox.Text.Trim(),
                    Email = string.IsNullOrWhiteSpace(ClientEmailBox.Text) ? null : ClientEmailBox.Text.Trim(),
                    PassportSeries = string.IsNullOrWhiteSpace(PassportSeriesBox.Text) ? null : PassportSeriesBox.Text.Trim(),
                    PassportNumber = string.IsNullOrWhiteSpace(PassportNumberBox.Text) ? null : PassportNumberBox.Text.Trim(),
                    RegistrationDate = DateOnly.FromDateTime(DateTime.Now)
                };

                App.DbContext.Clients.Add(client);
            }

            return client;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}