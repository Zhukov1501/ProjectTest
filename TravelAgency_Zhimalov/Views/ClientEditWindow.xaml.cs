using System;
using System.Windows;
using TravelAgencyApp.Data;
using TravelAgencyApp.Models;

namespace TravelAgencyApp.Views;

public partial class ClientEditWindow : Window
{
    private readonly DatabaseService _db;
    private readonly Client? _client;
    private readonly bool _isEdit;

    public ClientEditWindow(Client? client)
    {
        InitializeComponent();
        _db = DatabaseService.Instance;
        _client = client;
        _isEdit = client != null;

        if (_isEdit && client != null)
        {
            txtTitle.Text = "Редактировать клиента";
            btnDelete.Visibility = Visibility.Visible;
            LoadClientData(client);
        }

        txtLastName.Focus();
    }

    private void LoadClientData(Client client)
    {
        txtLastName.Text = client.LastName;
        txtFirstName.Text = client.FirstName;
        txtMiddleName.Text = client.MiddleName;
        txtPassport.Text = client.PassportNumber;
        txtPhone.Text = client.Phone;
        txtEmail.Text = client.Email;
        txtAddress.Text = client.Address;
        txtNotes.Text = client.Notes;

        if (client.BirthDate != default)
            dpBirthDate.SelectedDate = client.BirthDate;
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtLastName.Text) || string.IsNullOrWhiteSpace(txtFirstName.Text))
        {
            MessageBox.Show("Заполните обязательные поля (Фамилия, Имя)", "Ошибка", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var client = _client ?? new Client();
        client.LastName = txtLastName.Text.Trim();
        client.FirstName = txtFirstName.Text.Trim();
        client.MiddleName = txtMiddleName.Text.Trim();
        client.PassportNumber = txtPassport.Text.Trim();
        client.Phone = txtPhone.Text.Trim();
        client.Email = txtEmail.Text.Trim();
        client.Address = txtAddress.Text.Trim();
        client.Notes = txtNotes.Text.Trim();

        if (dpBirthDate.SelectedDate.HasValue)
            client.BirthDate = dpBirthDate.SelectedDate.Value;

        try
        {
            if (_isEdit)
            {
                _db.UpdateClient(client);
            }
            else
            {
                _db.AddClient(client);
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
        if (_client == null) return;

        var result = MessageBox.Show(
            $"Удалить клиента \"{_client.FullName}\"?",
            "Подтверждение",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _db.DeleteClient(_client.Id);
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
