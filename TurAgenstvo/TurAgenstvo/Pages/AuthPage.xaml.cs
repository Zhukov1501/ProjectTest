using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TurAgenstvo.Model;

namespace TurAgenstvo.Pages
{
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusText.Text = "";

                string login = LoginBox.Text.Trim();
                string password = PassBox.Password;

                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    StatusText.Text = "Заполните все поля!";
                    return;
                }

                // Ищем сотрудника
                var employee = App.DbContext.Employees
                    .Include(e => e.Position)
                    .FirstOrDefault(e => e.Login == login && e.Password == password && e.IsActive == true);

                if (employee != null)
                {
                    MainWindow.CurrentEmployee = employee;

                    var mainWindow = Application.Current.MainWindow as MainWindow;
                    mainWindow?.UpdateUserInfo();

                    // Определяем роль по названию должности
                    string positionName = employee.Position?.PositionName?.ToLower() ?? "";

                    if (positionName.Contains("администратор"))
                    {
                        NavigationService.Navigate(new AdminMenuPage());
                    }
                    else if (positionName.Contains("менеджер"))
                    {
                        NavigationService.Navigate(new BookingsPage());
                    }
                    else if (positionName.Contains("бухгалтер"))
                    {
                        NavigationService.Navigate(new FinancePage());
                    }
                    else
                    {
                        StatusText.Text = "Неизвестная роль пользователя";
                    }
                }
                else
                {
                    StatusText.Text = "Неверный логин или пароль!";
                    PassBox.Password = "";
                    LoginBox.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при авторизации: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }
    }
}