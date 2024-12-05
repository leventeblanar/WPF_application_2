using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace YourNamespace
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadTableButtons();
        }

        private void LoadTableButtons()
        {
            try
            {
                // Táblanevek lekérdezése az adatbázisból
                var tableNames = DatabaseHelper.GetTableNames();

                // Fordítási szótár: ékezet nélküli név -> ékezetes név
                var tableNameTranslations = new Dictionary<string, string>
                {
                    { "Kutyak", "Kutyák" },
                    { "Gazdak", "Gazdák" },
                    { "Fajtak", "Fajták" },
                    { "EgeszsegugyiNyilvantartas", "Egészségügyi Nyilvántartás" },
                    { "Esemenyek", "Események" }
                };

                foreach (var tableName in tableNames)
                {
                    // Az ékezetes név megkeresése a fordítási szótárból
                    var displayName = tableNameTranslations.ContainsKey(tableName)
                        ? tableNameTranslations[tableName]
                        : tableName; // Ha nincs a szótárban, az eredeti nevet használjuk

                    // Gomb létrehozása
                    var button = new Button
                    {
                        Content = displayName,
                        Height = 40,
                        Margin = new Thickness(5),
                    };

                    // Esemény hozzáadása a gombhoz
                    button.Click += (s, e) => OnTableButtonClick(tableName);

                    // Gomb hozzáadása a StackPanelhez
                    ButtonContainer.Children.Add(button);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a táblák betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnTableButtonClick(string tableName)
        {
            if (tableName == "Kutyak")
            {
                var kutyaWindow = new KutyaWindow();
                kutyaWindow.ShowDialog();
            }
            else if (tableName == "Gazdak")
            {
                var gazdakWindow = new GazdakWindow();
                gazdakWindow.ShowDialog();
            }
            else if (tableName == "Fajtak")
            {
                var fajtakWindow = new FajtakWindow();
                fajtakWindow.ShowDialog();
            }
            else if (tableName == "EgeszsegugyiNyilvantartas")
            {
                var egeszsegugyiWindow = new EgeszsegugyiNyilvantartasWindow();
                egeszsegugyiWindow.ShowDialog();
            }
            else if (tableName == "Esemenyek")
            {
                var esemenyekWindow = new EsemenyekWindow();
                esemenyekWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show($"A(z) {tableName} tábla jelenleg nem támogatott!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
