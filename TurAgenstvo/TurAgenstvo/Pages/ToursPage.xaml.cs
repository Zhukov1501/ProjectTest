using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TurAgenstvo.Model;


namespace TurAgenstvo.Pages
{
    public partial class ToursPage : Page
    {
        public ToursPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTours();
        }

        private void LoadTours()
        {
            try
            {
                var tours = App.DbContext.Tours
                    .Include(t => t.Country)
                    .Include(t => t.City)
                    .Include(t => t.Hotel)
                        .ThenInclude(h => h.MealType)
                    .Where(t => t.IsActive == true)
                    .ToList();

                ToursGrid.ItemsSource = tours;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки туров: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AddTourButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddTourWindow();
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                LoadTours();
            }
        }

        private void ManageHotelsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HotelsPage());
        }

        private void EditTourButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Редактирование тура будет доступно в следующей версии",
                          "Информация",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }

        private void DeleteTourButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag == null) return;

            var tourId = (Guid)button.Tag;

            var result = MessageBox.Show("Вы уверены, что хотите удалить тур?",
                                        "Подтверждение",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var tour = App.DbContext.Tours.Find(tourId);
                    if (tour != null)
                    {
                        tour.IsActive = false;
                        App.DbContext.SaveChanges();
                        LoadTours();
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
    }
}