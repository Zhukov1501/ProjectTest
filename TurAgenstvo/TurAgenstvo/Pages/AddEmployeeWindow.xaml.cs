using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TurAgenstvo.Model;

namespace TurAgenstvo.Pages
{
    public partial class AddEmployeeWindow : Window
    {
        private List<Position> _positions;

        public AddEmployeeWindow(List<Position> positions)
        {
            InitializeComponent();
            _positions = positions;

            PositionComboBox.ItemsSource = _positions;
            PositionComboBox.DisplayMemberPath = "PositionName";
            PositionComboBox.SelectedValuePath = "PositionId";

            if (_positions.Count > 0)
                PositionComboBox.SelectedIndex = 0;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (PositionComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(LastNameBox.Text) ||
                string.IsNullOrWhiteSpace(FirstNameBox.Text) ||
                string.IsNullOrWhiteSpace(LoginBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                MessageBox.Show("Заполните все обязательные поля!",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return;
            }

            try
            {
                var employee = new Employee
                {
                    EmployeeId = Guid.NewGuid(),
                    LastName = LastNameBox.Text.Trim(),
                    FirstName = FirstNameBox.Text.Trim(),
                    MiddleName = string.IsNullOrWhiteSpace(MiddleNameBox.Text) ? null : MiddleNameBox.Text.Trim(),
                    PositionId = (int)PositionComboBox.SelectedValue,
                    Login = LoginBox.Text.Trim(),
                    Password = PasswordBox.Password,
                    Email = string.IsNullOrWhiteSpace(EmailBox.Text) ? null : EmailBox.Text.Trim(),
                    Phone = string.IsNullOrWhiteSpace(PhoneBox.Text) ? null : PhoneBox.Text.Trim(),
                    HireDate = DateOnly.FromDateTime(DateTime.Now),
                    IsActive = true
                };

                App.DbContext.Employees.Add(employee);
                App.DbContext.SaveChanges();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}