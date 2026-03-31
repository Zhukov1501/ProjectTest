using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using TurAgenstvo.Model;

namespace TurAgenstvo.Pages
{
    public partial class AddTourWindow : Window
    {
        public AddTourWindow()
        {
            InitializeComponent();
            LoadCountries();
        }

        private void LoadCountries()
        {
            try
            {
                var countries = App.DbContext.Countries.OrderBy(c => c.CountryName).ToList();
                // #region agent log
                System.IO.File.AppendAllText(@"debug-378db9.log", "{\"sessionId\":\"378db9\",\"hypothesisId\":\"H-1\",\"location\":\"AddTourWindow.cs:LoadCountries\",\"message\":\"Countries loaded\",\"data\":{\"count\":" + countries.Count + ",\"first\":\"" + (countries.FirstOrDefault()?.CountryName ?? "none") + "\"},\"timestamp\":" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "}\n");
                // #endregion
                CountryComboBox.ItemsSource = countries;
            }
            catch (Exception ex)
            {
                // #region agent log
                System.IO.File.AppendAllText(@"debug-378db9.log", "{\"sessionId\":\"378db9\",\"hypothesisId\":\"H-4\",\"location\":\"AddTourWindow.cs:LoadCountries:catch\",\"message\":\"EXCEPTION loading countries\",\"data\":{\"error\":\"" + ex.Message.Replace("\"","'") + "\"},\"timestamp\":" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "}\n");
                // #endregion
                MessageBox.Show($"Ошибка загрузки стран: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CountryComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CityComboBox.ItemsSource = null;
            HotelComboBox.ItemsSource = null;

            if (CountryComboBox.SelectedValue is int countryId)
            {
                var cities = App.DbContext.Cities
                    .Where(c => c.CountryId == countryId)
                    .OrderBy(c => c.CityName)
                    .ToList();
                CityComboBox.ItemsSource = cities;
            }
        }

        private void CityComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            HotelComboBox.ItemsSource = null;

            if (CityComboBox.SelectedValue is int cityId)
            {
                var hotels = App.DbContext.Hotels
                    .Where(h => h.CityId == cityId)
                    .OrderBy(h => h.HotelName)
                    .ToList();
                HotelComboBox.ItemsSource = hotels;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TourNameBox.Text) ||
                CountryComboBox.SelectedValue == null ||
                CityComboBox.SelectedValue == null ||
                DepartureDatePicker.SelectedDate == null ||
                ReturnDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Заполните все обязательные поля (*)!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ReturnDatePicker.SelectedDate <= DepartureDatePicker.SelectedDate)
            {
                MessageBox.Show("Дата возврата должна быть позже даты вылета!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(PriceBox.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Укажите корректную цену!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(MaxTouristsBox.Text, out int maxTourists) || maxTourists < 1)
            {
                MessageBox.Show("Укажите корректное количество мест (минимум 1)!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // #region agent log
                var hotelRaw = HotelComboBox.SelectedValue;
                System.IO.File.AppendAllText(@"debug-378db9.log", "{\"sessionId\":\"378db9\",\"hypothesisId\":\"H-2\",\"location\":\"AddTourWindow.cs:SaveButton\",\"message\":\"Saving tour\",\"data\":{\"hotelSelectedValueType\":\"" + (hotelRaw?.GetType()?.Name ?? "null") + "\",\"hotelAsInt\":\"" + (hotelRaw as int?).ToString() + "\",\"hotelCast\":\"" + (hotelRaw != null ? ((int?)hotelRaw).ToString() : "null") + "\"},\"timestamp\":" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "}\n");
                // #endregion
                var tour = new Tour
                {
                    TourId = Guid.NewGuid(),
                    TourName = TourNameBox.Text.Trim(),
                    CountryId = (int)CountryComboBox.SelectedValue,
                    CityId = (int)CityComboBox.SelectedValue,
                    HotelId = HotelComboBox.SelectedValue as int?,
                    DepartureDate = DateOnly.FromDateTime(DepartureDatePicker.SelectedDate!.Value),
                    ReturnDate = DateOnly.FromDateTime(ReturnDatePicker.SelectedDate!.Value),
                    Price = price,
                    MaxTourists = maxTourists,
                    Description = string.IsNullOrWhiteSpace(DescriptionBox.Text) ? null : DescriptionBox.Text.Trim(),
                    IsActive = IsActiveCheck.IsChecked == true
                };

                App.DbContext.Tours.Add(tour);
                App.DbContext.SaveChanges();

                MessageBox.Show($"Тур «{tour.TourName}» успешно добавлен!",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении тура: {ex.Message}",
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
