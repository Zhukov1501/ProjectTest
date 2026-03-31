using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using System.Windows;
using TurAgenstvo.Model;


namespace TurAgenstvo
{
    public partial class App : Application
    {
        // Статическое свойство для доступа к БД
        public static TurAgenstvoContext DbContext { get; private set; } = null!;

        // ПРАВИЛЬНОЕ переопределение OnStartup
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);  // ВАЖНО: вызываем базовый метод

            try
            {
                // Читаем строку подключения из App.config
                var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show("Строка подключения не найдена в App.config",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(1);  // Используем Environment.Exit вместо Shutdown
                    return;
                }

                // Создаем опции контекста
                var optionsBuilder = new DbContextOptionsBuilder<TurAgenstvoContext>();
                optionsBuilder.UseNpgsql(connectionString);

                // Инициализируем контекст
                DbContext = new TurAgenstvoContext(optionsBuilder.Options);

                // Проверяем подключение
                if (DbContext.Database.CanConnect())
                {
                    System.Diagnostics.Debug.WriteLine("✅ Подключение к БД успешно!");
                }
                else
                {
                    MessageBox.Show("❌ Не удалось подключиться к базе данных",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            DbContext?.Dispose();
            base.OnExit(e);
        }
    }
}