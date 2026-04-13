using System;
using System.Windows;
using System.Windows.Controls;
using TravelAgencyApp.Data;
using TravelAgencyApp.Models;

namespace TravelAgencyApp.Views;

public partial class TourEditWindow : Window
{
    private readonly DatabaseService _db;
    private readonly Tour? _tour;
    private readonly bool _isEdit;

    public TourEditWindow(Tour? tour)
    {
        InitializeComponent();
        _db = DatabaseService.Instance;
        _tour = tour;
        _isEdit = tour != null;

        if (_isEdit && tour != null)
        {
            txtTitle.Text = "Редактировать тур";
            btnDelete.Visibility = Visibility.Visible;
            LoadTourData(tour);
        }

        txtName.Focus();
    }

    private void LoadTourData(Tour tour)
    {
        txtName.Text = tour.Name;
        txtDescription.Text = tour.Description;
        txtCountry.Text = tour.Country;
        txtCity.Text = tour.City;
        cmbTourType.Text = tour.TourType;
        txtPrice.Text = tour.Price.ToString();
        txtDurationDays.Text = tour.DurationDays.ToString();
        txtMaxParticipants.Text = tour.MaxParticipants.ToString();
        txtHotelName.Text = tour.HotelName;
        txtAvailableSlots.Text = tour.AvailableSlots.ToString();
        chkIsAvailable.IsChecked = tour.IsAvailable;

        foreach (ComboBoxItem item in cmbHotelStars.Items)
        {
            if (item.Content?.ToString() == tour.HotelStars)
            {
                cmbHotelStars.SelectedItem = item;
                break;
            }
        }

        foreach (ComboBoxItem item in cmbMealType.Items)
        {
            if (item.Content?.ToString() == tour.MealType)
            {
                cmbMealType.SelectedItem = item;
                break;
            }
        }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Заполните обязательные поля (Название тура)", "Ошибка", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(txtCountry.Text))
        {
            MessageBox.Show("Заполните обязательные поля (Страна)", "Ошибка", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(txtPrice.Text, out var price) || price < 0)
        {
            MessageBox.Show("Введите корректную цену", "Ошибка", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var tour = _tour ?? new Tour();
        tour.Name = txtName.Text.Trim();
        tour.Description = txtDescription.Text.Trim();
        tour.Country = txtCountry.Text.Trim();
        tour.City = txtCity.Text.Trim();
        tour.TourType = cmbTourType.Text.Trim();
        tour.Price = price;
        tour.DurationDays = int.TryParse(txtDurationDays.Text, out var dur) ? dur : 1;
        tour.MaxParticipants = int.TryParse(txtMaxParticipants.Text, out var max) ? max : 10;
        tour.AvailableSlots = int.TryParse(txtAvailableSlots.Text, out var slots) ? slots : 10;
        tour.HotelName = txtHotelName.Text.Trim();
        tour.HotelStars = (cmbHotelStars.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "3*";
        tour.MealType = (cmbMealType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Завтрак";
        tour.IsAvailable = chkIsAvailable.IsChecked == true;

        try
        {
            if (_isEdit)
            {
                _db.UpdateTour(tour);
            }
            else
            {
                _db.AddTour(tour);
            }

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (_tour == null) return;

        var result = MessageBox.Show(
            $"Удалить тур \"{_tour.Name}\"?",
            "Подтверждение",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _db.DeleteTour(_tour.Id);
            DialogResult = true;
            Close();
        }
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
