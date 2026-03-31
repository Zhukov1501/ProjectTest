using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TurAgenstvo.Model;

namespace TurAgenstvo.Pages
{
    public partial class EmployeesPage : Page
    {
        public EmployeesPage()
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
                var employees = App.DbContext.Employees
                    .Include(e => e.Position)
                    .Where(e => e.IsActive == true)
                    .ToList();

                // #region agent log
                var posCol = PositionColumn as System.Windows.Controls.DataGridComboBoxColumn;
                System.IO.File.AppendAllText(@"debug-378db9.log", "{\"sessionId\":\"378db9\",\"hypothesisId\":\"H-C\",\"location\":\"EmployeesPage.xaml.cs:LoadData\",\"message\":\"EmployeesGrid loaded\",\"data\":{\"employeeCount\":" + employees.Count + ",\"posColItemsSourceIsNull\":" + (posCol?.ItemsSource == null ? "true" : "false") + "},\"timestamp\":" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "}\n");
                // #endregion

                EmployeesGrid.ItemsSource = employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            var positions = App.DbContext.Positions.ToList();
            var addWindow = new AddEmployeeWindow(positions);
            addWindow.Owner = Application.Current.MainWindow;
            if (addWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.DbContext.SaveChanges();
                MessageBox.Show("Изменения сохранены!",
                              "Успех",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void DeleteEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            // #region agent log
            System.IO.File.AppendAllText(@"debug-378db9.log", "{\"sessionId\":\"378db9\",\"hypothesisId\":\"H-B\",\"location\":\"EmployeesPage.xaml.cs:DeleteEmployeeButton_Click\",\"message\":\"Delete clicked\",\"data\":{\"tagType\":\"" + (button?.Tag?.GetType()?.Name ?? "null") + "\",\"tagValue\":\"" + (button?.Tag?.ToString() ?? "null") + "\"},\"timestamp\":" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "}\n");
            // #endregion
            if (button?.Tag == null) return;

            var employeeId = (Guid)button.Tag;

            var result = MessageBox.Show("Вы уверены, что хотите удалить сотрудника?",
                                        "Подтверждение",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var employee = App.DbContext.Employees.Find(employeeId);
                    if (employee != null)
                    {
                        employee.IsActive = false;
                        App.DbContext.SaveChanges();
                        LoadData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}",
                                  "Ошибка",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
        }

        // ДОБАВЬТЕ ЭТОТ МЕТОД:
        private void EmployeesGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            // Этот метод вызывается при завершении редактирования строки
            // Можно оставить пустым, если не нужна дополнительная логика
            // Изменения будут сохранены при нажатии кнопки "Сохранить"
        }
    }
}