using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TurAgenstvo.Model;

namespace TurAgenstvo.Pages
{
    public partial class BookingsPage : Page
    {
        public BookingsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBookings();
        }

        private void LoadBookings()
        {
            // #region agent log
            System.IO.File.AppendAllText(@"debug-378db9.log", "{\"sessionId\":\"378db9\",\"hypothesisId\":\"H-A\",\"location\":\"BookingsPage.xaml.cs:LoadBookings\",\"message\":\"LoadBookings called\",\"data\":{\"bookingsGridIsNull\":" + (BookingsGrid == null ? "true" : "false") + ",\"statusFilterIsNull\":" + (StatusFilter == null ? "true" : "false") + "},\"timestamp\":" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "}\n");
            // #endregion

            if (BookingsGrid == null) return;

            try
            {
                var query = App.DbContext.Bookings
                    .Include(b => b.Client)
                    .Include(b => b.Tour)
                        .ThenInclude(t => t.Hotel)
                            .ThenInclude(h => h.MealType)
                    .Include(b => b.Employee)
                    .AsQueryable();

                var selectedFilter = (StatusFilter?.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString();
                if (!string.IsNullOrEmpty(selectedFilter) && selectedFilter != "Все")
                    query = query.Where(b => b.Status == selectedFilter);

                BookingsGrid.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки бронирований: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.CurrentEmployee = null;
            NavigationService.Navigate(new AuthPage());
        }

        private void NewBookingButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddBookingWindow();
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                LoadBookings();
            }
        }

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadBookings();
        }

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag == null) return;

            var bookingId = (Guid)button.Tag;

            try
            {
                var booking = App.DbContext.Bookings.Find(bookingId);
                if (booking != null)
                {
                    booking.Status = "Оплачено";

                    var payment = new Payment
                    {
                        PaymentId = Guid.NewGuid(),
                        BookingId = bookingId,
                        Amount = booking.TotalPrice,
                        PaymentDate = DateTime.Now,
                        PaymentMethod = "Наличные",
                        PaymentStatus = "Оплачено",
                        EmployeeId = MainWindow.CurrentEmployee?.EmployeeId
                    };

                    App.DbContext.Payments.Add(payment);
                    App.DbContext.SaveChanges();

                    LoadBookings();

                    MessageBox.Show("Платеж успешно проведен!",
                                  "Успех",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проведении платежа: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }
    }
}