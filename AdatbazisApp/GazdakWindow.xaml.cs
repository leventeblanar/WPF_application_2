using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;

namespace YourNamespace
{
    public partial class GazdakWindow : Window
    {
        private const string ConnectionString = "Data Source=doggo_manager.db";
        private DataRowView? _selectedRow;

        public GazdakWindow()
        {
            InitializeComponent();
            LoadGazdakData();
        }

        // Adatok betöltése
        private void LoadGazdakData()
        {
            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Gazdak";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            var dataTable = new DataTable();
                            dataTable.Load(reader);
                            GazdakDataGrid.ItemsSource = dataTable.DefaultView;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba az adatok betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Hozzáadás
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedRow = null;
            ShowForm();
        }

        // Módosítás
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (GazdakDataGrid.SelectedItem is DataRowView selectedRow)
            {
                _selectedRow = selectedRow;
                ShowForm();

                NameTextBox.Text = _selectedRow["Nev"].ToString();
                AddressTextBox.Text = _selectedRow["Cim"].ToString();
                PhoneTextBox.Text = _selectedRow["Telefonszam"].ToString();
            }
            else
            {
                MessageBox.Show("Válassz ki egy sort a módosításhoz!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Törlés
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (GazdakDataGrid.SelectedItem is DataRowView selectedRow)
            {
                var result = MessageBox.Show("Biztosan törölni szeretnéd ezt a sort?", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var connection = new SqliteConnection(ConnectionString))
                        {
                            connection.Open();
                            string query = "DELETE FROM Gazdak WHERE ID = @ID";

                            using (var command = new SqliteCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@ID", selectedRow["ID"]);
                                command.ExecuteNonQuery();
                                MessageBox.Show("A sor sikeresen törölve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadGazdakData();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Hiba a törlés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Válassz ki egy sort a törléshez!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Mentés
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();

                    string query;
                    if (_selectedRow == null)
                    {
                        query = "INSERT INTO Gazdak (Nev, Cim, Telefonszam) VALUES (@Nev, @Cim, @Telefonszam)";
                    }
                    else
                    {
                        query = "UPDATE Gazdak SET Nev = @Nev, Cim = @Cim, Telefonszam = @Telefonszam WHERE ID = @ID";
                    }

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nev", NameTextBox.Text);
                        command.Parameters.AddWithValue("@Cim", AddressTextBox.Text);
                        command.Parameters.AddWithValue("@Telefonszam", PhoneTextBox.Text);

                        if (_selectedRow != null)
                        {
                            command.Parameters.AddWithValue("@ID", _selectedRow["ID"]);
                        }

                        command.ExecuteNonQuery();
                        MessageBox.Show("Adatok sikeresen mentve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                        HideForm();
                        LoadGazdakData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba mentés közben: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Form megjelenítése
        private void ShowForm()
        {
            FormPanel.Visibility = Visibility.Visible;
        }

        // Form elrejtése
        private void HideForm()
        {
            FormPanel.Visibility = Visibility.Collapsed;
            NameTextBox.Clear();
            AddressTextBox.Clear();
            PhoneTextBox.Clear();
        }
    }
}
