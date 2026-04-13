using System.Windows;
using System.Windows.Input;
using TravelAgencyApp.Data;
using TravelAgencyApp.Models;

namespace TravelAgencyApp.Views;

public partial class LoginWindow : Window
{
    private readonly DatabaseService _db;

    public LoginWindow()
    {
        InitializeComponent();
        _db = DatabaseService.Instance;
        txtUsername.Focus();
    }

    private void btnLogin_Click(object sender, RoutedEventArgs e)
    {
        PerformLogin();
    }

    private void txtUsername_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            PerformLogin();
    }

    private void txtPassword_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            PerformLogin();
    }

    private void chkShowPassword_Changed(object sender, RoutedEventArgs e)
    {
        if (chkShowPassword.IsChecked == true)
        {
            txtPasswordVisible.Text = txtPassword.Password;
            txtPasswordVisible.Visibility = Visibility.Visible;
            txtPassword.Visibility = Visibility.Collapsed;
        }
        else
        {
            txtPassword.Password = txtPasswordVisible.Text;
            txtPassword.Visibility = Visibility.Visible;
            txtPasswordVisible.Visibility = Visibility.Collapsed;
        }
    }

    private void PerformLogin()
    {
        string username = txtUsername.Text.Trim();
        string password = chkShowPassword.IsChecked == true ? txtPasswordVisible.Text : txtPassword.Password;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowError("Введите логин и пароль");
            return;
        }

        User? user = _db.ValidateUser(username, password);

        if (user != null)
        {
            Session.CurrentUser = user;
            txtError.Visibility = Visibility.Collapsed;

            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
        else
        {
            ShowError("Неверный логин или пароль");
            txtPassword.Password = "";
            txtPassword.Focus();
        }
    }

    private void ShowError(string message)
    {
        txtError.Text = message;
        txtError.Visibility = Visibility.Visible;
    }
}

public static class Session
{
    public static User? CurrentUser { get; set; }
    public static bool IsAuthenticated => CurrentUser != null;
    public static bool IsAdmin => CurrentUser?.Role == "Admin";
    public static bool IsManager => CurrentUser?.Role == "Admin" || CurrentUser?.Role == "Manager";
}
