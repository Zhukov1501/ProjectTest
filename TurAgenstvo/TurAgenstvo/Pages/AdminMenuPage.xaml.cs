using System.Windows.Controls;  // Важно: using для Page
using TurAgenstvo.Pages;

namespace TurAgenstvo.Pages
{
    public partial class AdminMenuPage : Page  // Должно быть : Page, не : Window
    {
        public AdminMenuPage()
        {
            InitializeComponent();
        }

        private void EmployeesButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new EmployeesPage());
        }

        private void ToursButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new ToursPage());
        }

        private void FinanceButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new FinancePage());
        }
    }
}