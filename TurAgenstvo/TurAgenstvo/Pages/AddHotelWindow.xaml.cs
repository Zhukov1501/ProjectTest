using System;
using System.Linq;
using System.Windows;
using TurAgenstvo.Model;

namespace TurAgenstvo.Pages
{
    public partial class AddHotelWindow : Window
    {
        public AddHotelWindow()
        {
            InitializeComponent();
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            var countries = App.DbContext.Countries.OrderBy(c => c.CountryName).ToList();
            CountryComboBox.ItemsSource = countries;

            var mealTypes = App.DbContext.MealTypes.OrderBy(m => m.MealName).ToList();
            MealTypeComboBox.ItemsSource = mealTypes;
        }

        private void CountryComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CityComboBox.ItemsSource = null;

            if (CountryComboBox.SelectedValue is int countryId)
            {
                var cities = App.DbContext.Cities
                    .Where(c => c.CountryId == countryId)
                    .OrderBy(c => c.CityName)
                    .ToList();
                CityComboBox.ItemsSource = cities;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(HotelNameBox.Text) ||
                CountryComboBox.SelectedValue == null ||
                CityComboBox.SelectedValue == null)
            {
                MessageBox.Show("Заполните все обязательные поля (*)!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int stars = 3;
            if (StarsComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem item &&
                int.TryParse(item.Content?.ToString(), out int s))
                stars = s;

            try
            {
                var hotel = new Hotel
                {
                    HotelName = HotelNameBox.Text.Trim(),
                    CityId = (int)CityComboBox.SelectedValue,
                    Stars = stars,
                    MealtypeId = MealTypeComboBox.SelectedValue as int?,
                    Address = string.IsNullOrWhiteSpace(AddressBox.Text) ? null : AddressBox.Text.Trim(),
                    Phone = string.IsNullOrWhiteSpace(PhoneBox.Text) ? null : PhoneBox.Text.Trim(),
                    Email = string.IsNullOrWhiteSpace(EmailBox.Text) ? null : EmailBox.Text.Trim(),
                    Description = string.IsNullOrWhiteSpace(DescriptionBox.Text) ? null : DescriptionBox.Text.Trim()
                };

                App.DbContext.Hotels.Add(hotel);
                App.DbContext.SaveChanges();

                MessageBox.Show($"Отель «{hotel.HotelName}» успешно добавлен!",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении отеля: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
