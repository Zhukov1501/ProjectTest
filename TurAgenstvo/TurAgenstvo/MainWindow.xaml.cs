using System.Windows;
using System.Windows.Controls;
using TurAgenstvo.Pages;
using TurAgenstvo.Model;

namespace TurAgenstvo
{
    public partial class MainWindow : Window
    {
        // Текущий сотрудник
        public static Employee CurrentEmployee { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new AuthPage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentEmployee = null;
            MainFrame.Navigate(new AuthPage());
            UpdateUserInfo();
        }

        public void UpdateUserInfo()
        {
            if (CurrentEmployee != null && UserInfoText != null)
            {
                UserInfoText.Text = $"Вы вошли как: {CurrentEmployee.LastName} {CurrentEmployee.FirstName}";
            }
        }
    }
}