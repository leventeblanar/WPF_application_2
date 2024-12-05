using System;
using System.Data;
using System.Windows;
using Microsoft.Data.Sqlite;

namespace YourNamespace
{
    public partial class FajtakWindow : Window
    {
        private const string ConnectionString = "Data Source=doggo_manager.db";
        private DataRowView? _selectedRow;

        public FajtakWindow()
        {
            InitializeComponent();
            LoadFajtakData();
        }

        // Adatok betöltése
        private void LoadFajtakData()
        {
            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Fajtak";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            var dataTable = new DataTable();
                            dataTable.Load(reader);
                            FajtakDataGrid.ItemsSource = dataTable.DefaultView;
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
            if (FajtakDataGrid.SelectedItem is DataRowView selectedRow)
            {
                _selectedRow = selectedRow;
                ShowForm();

                NameTextBox.Text = _selectedRow["FajtaNev"].ToString();
                DescriptionTextBox.Text = _selectedRow["Leiras"].ToString();
            }
            else
            {
                MessageBox.Show("Válassz ki egy sort a módosításhoz!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Törlés
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (FajtakDataGrid.SelectedItem is DataRowView selectedRow)
            {
                var result = MessageBox.Show("Biztosan törölni szeretnéd ezt a sort?", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var connection = new SqliteConnection(ConnectionString))
                        {
                            connection.Open();
                            string query = "DELETE FROM Fajtak WHERE ID = @ID";

                            using (var command = new SqliteCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@ID", selectedRow["ID"]);
                                command.ExecuteNonQuery();
                                MessageBox.Show("A sor sikeresen törölve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadFajtakData();
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
                        query = "INSERT INTO Fajtak (FajtaNev, Leiras) VALUES (@FajtaNev, @Leiras)";
                    }
                    else
                    {
                        query = "UPDATE Fajtak SET FajtaNev = @FajtaNev, Leiras = @Leiras WHERE ID = @ID";
                    }

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FajtaNev", NameTextBox.Text);
                        command.Parameters.AddWithValue("@Leiras", DescriptionTextBox.Text);

                        if (_selectedRow != null)
                        {
                            command.Parameters.AddWithValue("@ID", _selectedRow["ID"]);
                        }

                        command.ExecuteNonQuery();
                        MessageBox.Show("Adatok sikeresen mentve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                        HideForm();
                        LoadFajtakData();
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
            DescriptionTextBox.Clear();
        }
    }
}
