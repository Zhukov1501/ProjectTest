using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TurAgenstvo.Model;

namespace TurAgenstvo.Pages
{
    public partial class FinancePage : Page
    {
        public FinancePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                LoadFinancialOperations();
                LoadStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void LoadFinancialOperations()
        {
            if (FinanceGrid == null) return;

            var query = App.DbContext.FinancialOperations
                .Include(f => f.Booking)
                    .ThenInclude(b => b.Client)
                .Include(f => f.Employee)
                .AsQueryable();

            var selectedPeriod = (PeriodFilter?.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(selectedPeriod) && selectedPeriod != "За все время")
            {
                var now = DateTime.Now;
                DateTime fromDate = selectedPeriod switch
                {
                    "За сегодня" => now.Date,
                    "За неделю" => now.Date.AddDays(-7),
                    "За месяц" => now.Date.AddMonths(-1),
                    "За год" => now.Date.AddYears(-1),
                    _ => DateTime.MinValue
                };
                query = query.Where(f => f.OperationDate >= fromDate);
            }

            FinanceGrid.ItemsSource = query.OrderByDescending(f => f.OperationDate).ToList();
        }

        private void LoadStatistics()
        {
            // Общая выручка
            var totalRevenue = App.DbContext.FinancialOperations
                .Where(f => f.OperationType == "Поступление")
                .Sum(f => (decimal?)f.Amount) ?? 0;
            TotalRevenueText.Text = $"{totalRevenue:N0} ₽";

            // Количество оплаченных броней
            var paidBookings = App.DbContext.Bookings
                .Count(b => b.Status == "Оплачено");
            PaidBookingsText.Text = paidBookings.ToString();

            // Количество броней, ожидающих оплаты
            var pendingBookings = App.DbContext.Bookings
                .Count(b => b.Status == "Подтверждено");
            PendingBookingsText.Text = pendingBookings.ToString();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.CurrentEmployee = null;
            NavigationService.Navigate(new AuthPage());
        }

        private void ProcessPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Проведение платежа будет доступно в следующей версии",
                          "Информация",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }

        private void PeriodFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadFinancialOperations();
        }

        private void FinanceGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var operation = FinanceGrid.SelectedItem as Financialoperation;
            if (operation != null)
            {
                string details = $"Операция: {operation.OperationNumber}\n" +
                                $"Дата: {operation.OperationDate:dd.MM.yyyy HH:mm}\n" +
                                $"Тип: {operation.OperationType}\n" +
                                $"Сумма: {operation.Amount:N0} ₽\n" +
                                $"Бронь: {operation.Booking?.BookingNumber}\n" +
                                $"Описание: {operation.Description}";

                MessageBox.Show(details, "Детали операции", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}