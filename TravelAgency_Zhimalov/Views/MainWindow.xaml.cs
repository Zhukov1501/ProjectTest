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
        LoadServices();
        LoadCategories();
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

    private void LoadServices()
    {
        if (dgServices == null) return;
        
        string? search = string.IsNullOrWhiteSpace(txtServiceSearch.Text) ? null : txtServiceSearch.Text;
        string? category = (cmbCategory.SelectedItem as ComboBoxItem)?.Content?.ToString();
        if (category == "Все категории") category = null;
        
        string? sortBy = (cmbServiceSort.SelectedItem as ComboBoxItem)?.Content?.ToString();
        bool ascending = chkServiceAsc?.IsChecked == true;

        decimal? minPrice = null;
        decimal? maxPrice = null;

        if (decimal.TryParse(txtMinPrice.Text, out var min))
            minPrice = min;
        if (decimal.TryParse(txtMaxPrice.Text, out var max))
            maxPrice = max;

        var services = _db.GetServices(search, category, sortBy, ascending, minPrice, maxPrice);
        dgServices.ItemsSource = services;
    }

    private void LoadCategories()
    {
        var categories = _db.GetCategories().ToList();
        cmbCategory.Items.Clear();
        cmbCategory.Items.Add(new ComboBoxItem { Content = "Все категории" });
        foreach (var category in categories)
        {
            cmbCategory.Items.Add(new ComboBoxItem { Content = category });
        }
        cmbCategory.SelectedIndex = 0;
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
        LoadServices();
    }

    private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoadServices();
    }

    private void cmbServiceSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoadServices();
    }

    private void chkServiceSort_Changed(object sender, RoutedEventArgs e)
    {
        LoadServices();
    }

    private void txtPrice_TextChanged(object sender, TextChangedEventArgs e)
    {
        LoadServices();
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
        if (dgServices.SelectedItem is AttractionService service)
        {
            var editWindow = new ServiceEditWindow(service);
            if (editWindow.ShowDialog() == true)
            {
                LoadServices();
                LoadCategories();
            }
        }
    }

    private void btnAddService_Click(object sender, RoutedEventArgs e)
    {
        var editWindow = new ServiceEditWindow(null);
        if (editWindow.ShowDialog() == true)
        {
            LoadServices();
            LoadCategories();
        }
    }
}
