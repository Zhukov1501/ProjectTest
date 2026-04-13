using System;
using System.Windows;
using System.Windows.Controls;
using TravelAgencyApp.Data;
using TravelAgencyApp.Models;

namespace TravelAgencyApp.Views;

public partial class ServiceEditWindow : Window
{
    private readonly DatabaseService _db;
    private readonly AttractionService? _service;
    private readonly bool _isEdit;

    public ServiceEditWindow(AttractionService? service)
    {
        InitializeComponent();
        _db = DatabaseService.Instance;
        _service = service;
        _isEdit = service != null;

        if (_isEdit && service != null)
        {
            txtTitle.Text = "Редактировать услугу";
            btnDelete.Visibility = Visibility.Visible;
            LoadServiceData(service);
        }

        txtName.Focus();
    }

    private void LoadServiceData(AttractionService service)
    {
        txtName.Text = service.Name;
        txtDescription.Text = service.Description;
        cmbCategory.Text = service.Category;
        txtPrice.Text = service.Price.ToString();
        txtDuration.Text = service.DurationMinutes.ToString();
        txtMaxParticipants.Text = service.MaxParticipants.ToString();
        txtAvailableSlots.Text = service.AvailableSlots.ToString();
        txtLocation.Text = service.Location;
        chkIsAvailable.IsChecked = service.IsAvailable;

        foreach (ComboBoxItem item in cmbAgeRestriction.Items)
        {
            if (item.Content?.ToString() == service.AgeRestriction)
            {
                cmbAgeRestriction.SelectedItem = item;
                break;
            }
        }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Заполните обязательные поля (Название)", "Ошибка", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(txtPrice.Text, out var price) || price < 0)
        {
            MessageBox.Show("Введите корректную цену", "Ошибка", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var service = _service ?? new AttractionService();
        service.Name = txtName.Text.Trim();
        service.Description = txtDescription.Text.Trim();
        service.Category = cmbCategory.Text.Trim();
        service.Price = price;
        service.DurationMinutes = int.TryParse(txtDuration.Text, out var dur) ? dur : 0;
        service.MaxParticipants = int.TryParse(txtMaxParticipants.Text, out var max) ? max : 1;
        service.AvailableSlots = int.TryParse(txtAvailableSlots.Text, out var slots) ? slots : 0;
        service.Location = txtLocation.Text.Trim();
        service.AgeRestriction = (cmbAgeRestriction.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "0+";
        service.IsAvailable = chkIsAvailable.IsChecked == true;

        try
        {
            if (_isEdit)
            {
                _db.UpdateService(service);
            }
            else
            {
                _db.AddService(service);
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
        if (_service == null) return;

        var result = MessageBox.Show(
            $"Удалить услугу \"{_service.Name}\"?",
            "Подтверждение",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _db.DeleteService(_service.Id);
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
