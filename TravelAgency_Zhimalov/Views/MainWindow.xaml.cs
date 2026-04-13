using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TravelAgencyApp.Data;
using TravelAgencyApp.Models;

namespace TravelAgencyApp.Views;

public partial class MainWindow : Window
{
    private readonly DatabaseService _db;

    public MainWindow()
    {
        InitializeComponent();
        _db = DatabaseService.Instance;

        if (Session.CurrentUser != null)
        {
            txtUserInfo.Text = $"{Session.CurrentUser.FullName} ({Session.CurrentUser.Role})";
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        bool canEdit = Session.IsManager;
        btnAddClient.Visibility = canEdit ? Visibility.Visible : Visibility.Collapsed;
        btnAddService.Visibility = canEdit ? Visibility.Visible : Visibility.Collapsed;
        dgClients.IsReadOnly = !canEdit;
        dgServices.IsReadOnly = !canEdit;
        
        LoadClients();
        LoadTours();
        LoadTourTypes();
        LoadCountries();
    }

    private void btnLogout_Click(object sender, RoutedEventArgs e)
    {
        Session.CurrentUser = null;
        var loginWindow = new LoginWindow();
        loginWindow.Show();
        this.Close();
    }

    private void LoadClients()
    {
        if (dgClients == null) return;
        
        string? search = string.IsNullOrWhiteSpace(txtClientSearch.Text) ? null : txtClientSearch.Text;
        string? sortBy = (cmbClientSort.SelectedItem as ComboBoxItem)?.Content?.ToString();
        bool ascending = chkClientAsc?.IsChecked == true;

        var clients = _db.GetClients(search, sortBy, ascending);
        dgClients.ItemsSource = clients;
    }

    private void LoadTours()
    {
        if (dgServices == null) return;
        
        string? search = string.IsNullOrWhiteSpace(txtServiceSearch.Text) ? null : txtServiceSearch.Text;
        string? tourType = (cmbTourType.SelectedItem as ComboBoxItem)?.Content?.ToString();
        if (tourType == "Все типы") tourType = null;
        
        string? country = (cmbCountry.SelectedItem as ComboBoxItem)?.Content?.ToString();
        if (country == "Все страны") country = null;
        
        string? sortBy = (cmbServiceSort.SelectedItem as ComboBoxItem)?.Content?.ToString();
        bool ascending = chkServiceAsc?.IsChecked == true;

        decimal? minPrice = null;
        decimal? maxPrice = null;

        if (decimal.TryParse(txtMinPrice.Text, out var min))
            minPrice = min;
        if (decimal.TryParse(txtMaxPrice.Text, out var max))
            maxPrice = max;

        var tours = _db.GetTours(search, tourType, country, sortBy, ascending, minPrice, maxPrice);
        dgServices.ItemsSource = tours;
    }

    private void LoadTourTypes()
    {
        var tourTypes = _db.GetTourTypes().ToList();
        cmbTourType.Items.Clear();
        cmbTourType.Items.Add(new ComboBoxItem { Content = "Все типы" });
        foreach (var type in tourTypes)
        {
            cmbTourType.Items.Add(new ComboBoxItem { Content = type });
        }
        cmbTourType.SelectedIndex = 0;
    }

    private void LoadCountries()
    {
        var countries = _db.GetCountries().ToList();
        cmbCountry.Items.Clear();
        cmbCountry.Items.Add(new ComboBoxItem { Content = "Все страны" });
        foreach (var country in countries)
        {
            cmbCountry.Items.Add(new ComboBoxItem { Content = country });
        }
        cmbCountry.SelectedIndex = 0;
    }

    private void txtClientSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        LoadClients();
    }

    private void cmbClientSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoadClients();
    }

    private void chkClientSort_Changed(object sender, RoutedEventArgs e)
    {
        LoadClients();
    }

    private void txtServiceSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        LoadTours();
    }

    private void cmbTourType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoadTours();
    }

    private void cmbCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoadTours();
    }

    private void cmbServiceSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoadTours();
    }

    private void chkServiceSort_Changed(object sender, RoutedEventArgs e)
    {
        LoadTours();
    }

    private void txtPrice_TextChanged(object sender, TextChangedEventArgs e)
    {
        LoadTours();
    }

    private void dgClients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (dgClients.SelectedItem is Client client)
        {
            var editWindow = new ClientEditWindow(client);
            if (editWindow.ShowDialog() == true)
            {
                LoadClients();
            }
        }
    }

    private void btnAddClient_Click(object sender, RoutedEventArgs e)
    {
        var editWindow = new ClientEditWindow(null);
        if (editWindow.ShowDialog() == true)
        {
            LoadClients();
        }
    }

    private void dgServices_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (dgServices.SelectedItem is Tour tour)
        {
            var editWindow = new TourEditWindow(tour);
            if (editWindow.ShowDialog() == true)
            {
                LoadTours();
                LoadTourTypes();
                LoadCountries();
            }
        }
    }

    private void btnAddTour_Click(object sender, RoutedEventArgs e)
    {
        var editWindow = new TourEditWindow(null);
        if (editWindow.ShowDialog() == true)
        {
            LoadTours();
            LoadTourTypes();
            LoadCountries();
        }
    }
}
