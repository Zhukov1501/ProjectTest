using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TurAgenstvo.Model;
using TurAgenstvo.Extensions;

namespace TurAgenstvo.Pages
{
    public partial class HotelsPage : Page
    {
        public HotelsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadHotels();
        }

        private void LoadHotels()
        {
            try
            {
                var hotels = App.DbContext.Hotels
                    .Include(h => h.City)
                        .ThenInclude(c => c.Country)
                    .Include(h => h.MealType)
                    .ToList();

                HotelsGrid.ItemsSource = hotels;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отелей: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AddHotelButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddHotelWindow();
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                LoadHotels();
            }
        }
    }
}