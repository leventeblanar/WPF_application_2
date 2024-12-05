using System;
using System.Data;
using System.Windows;
using Microsoft.Data.Sqlite;

namespace YourNamespace
{
    public partial class KutyaWindow : Window
    {
        private const string ConnectionString = "Data Source=doggo_manager.db";
        private DataRowView? _selectedRow;

        public KutyaWindow()
        {
            InitializeComponent();
            LoadKutyaData();
        }

        // Adatok betöltése
        private void LoadKutyaData()
        {
            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();

                    // SQL lekérdezés a Kutyák táblához, amely csatlakozik a Fajták táblához
                    string query = @"
                        SELECT 
                            Kutyak.ID, 
                            Kutyak.Nev, 
                            Fajtak.FajtaNev AS Fajta, 
                            Kutyak.Kor, 
                            Kutyak.GazdaID
                        FROM Kutyak
                        LEFT JOIN Fajtak ON Kutyak.Fajta = Fajtak.ID";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            var dataTable = new DataTable();
                            dataTable.Load(reader);
                            KutyaDataGrid.ItemsSource = dataTable.DefaultView;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba az adatok betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Fajták betöltése ComboBox-ba (betűrendben)
        private void LoadFajtakToComboBox()
        {
            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT ID, FajtaNev FROM Fajtak ORDER BY FajtaNev ASC";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            BreedComboBox.Items.Clear(); // Korábbi elemek törlése

                            while (reader.Read())
                            {
                                var fajtaId = reader["ID"].ToString();
                                var fajtaNev = reader["FajtaNev"].ToString();

                                // Hozzáadjuk az ID és név párost (pl. "1 - Labrador")
                                BreedComboBox.Items.Add($"{fajtaId} - {fajtaNev}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a fajták betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Gazdák betöltése ComboBox-ba
        private void LoadGazdakToComboBox()
        {
            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT ID, Nev FROM Gazdak";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            GazdaIDComboBox.Items.Clear(); // Korábbi elemek törlése

                            while (reader.Read())
                            {
                                var gazdaId = reader["ID"].ToString();
                                var gazdaNev = reader["Nev"].ToString();

                                // Hozzáadjuk az ID és név párost (pl. "1 - Kovács István")
                                GazdaIDComboBox.Items.Add($"{gazdaId} - {gazdaNev}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a gazdák betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Hozzáadás gomb eseménye
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedRow = null;
            ShowForm();
        }

        // Módosítás gomb eseménye
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (KutyaDataGrid.SelectedItem is DataRowView selectedRow)
            {
                _selectedRow = selectedRow;
                ShowForm();

                NameTextBox.Text = _selectedRow["Nev"].ToString();
                AgeTextBox.Text = _selectedRow["Kor"].ToString();

                // Fajta kiválasztása
                var fajtaNev = _selectedRow["Fajta"].ToString();
                foreach (var item in BreedComboBox.Items)
                {
                    if (item.ToString()?.Contains($"- {fajtaNev}") == true)
                    {
                        BreedComboBox.SelectedItem = item;
                        break;
                    }
                }

                // GazdaID kiválasztása
                var gazdaId = _selectedRow["GazdaID"].ToString();
                foreach (var item in GazdaIDComboBox.Items)
                {
                    if (item.ToString()?.StartsWith(gazdaId + " -") == true)
                    {
                        GazdaIDComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Válassz ki egy sort a módosításhoz!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Törlés gomb eseménye
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (KutyaDataGrid.SelectedItem is DataRowView selectedRow)
            {
                var result = MessageBox.Show("Biztosan törölni szeretnéd ezt a sort?", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var connection = new SqliteConnection(ConnectionString))
                        {
                            connection.Open();
                            string query = "DELETE FROM Kutyak WHERE ID = @ID";

                            using (var command = new SqliteCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@ID", selectedRow["ID"]);
                                command.ExecuteNonQuery();
                                MessageBox.Show("A sor sikeresen törölve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadKutyaData();
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

        // Mentés gomb eseménye
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
                        query = "INSERT INTO Kutyak (Nev, Fajta, Kor, GazdaID) VALUES (@Nev, @Fajta, @Kor, @GazdaID)";
                    }
                    else
                    {
                        query = "UPDATE Kutyak SET Nev = @Nev, Fajta = @Fajta, Kor = @Kor, GazdaID = @GazdaID WHERE ID = @ID";
                    }

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nev", string.IsNullOrEmpty(NameTextBox.Text) ? DBNull.Value : NameTextBox.Text);

                        if (BreedComboBox.SelectedItem != null)
                        {
                            var selectedFajta = BreedComboBox.SelectedItem.ToString();
                            var fajtaId = selectedFajta?.Split('-')[0].Trim();
                            command.Parameters.AddWithValue("@Fajta", string.IsNullOrEmpty(fajtaId) ? (object)DBNull.Value : fajtaId);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Fajta", DBNull.Value);
                        }

                        if (int.TryParse(AgeTextBox.Text, out var age))
                        {
                            command.Parameters.AddWithValue("@Kor", age);
                        }
                        else
                        {
                            MessageBox.Show("A 'Kor' mező érvényes szám kell legyen!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        if (GazdaIDComboBox.SelectedItem != null)
                        {
                            var selectedGazda = GazdaIDComboBox.SelectedItem.ToString();
                            var gazdaId = selectedGazda?.Split('-')[0].Trim();
                            command.Parameters.AddWithValue("@GazdaID", string.IsNullOrEmpty(gazdaId) ? (object)DBNull.Value : gazdaId);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@GazdaID", DBNull.Value);
                        }

                        if (_selectedRow != null)
                        {
                            command.Parameters.AddWithValue("@ID", _selectedRow["ID"]);
                        }

                        command.ExecuteNonQuery();
                        MessageBox.Show("Adatok sikeresen mentve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                        HideForm();
                        LoadKutyaData();
                    }
                }
            }
            catch (SqliteException ex)
            {
                MessageBox.Show($"Adatbázis hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ismeretlen hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Form megjelenítése
        private void ShowForm()
        {
            FormPanel.Visibility = Visibility.Visible;
            LoadFajtakToComboBox();
            LoadGazdakToComboBox();
        }

        // Form elrejtése
        private void HideForm()
        {
            FormPanel.Visibility = Visibility.Collapsed;
            NameTextBox.Clear();
            BreedComboBox.SelectedIndex = -1;
            AgeTextBox.Clear();
            GazdaIDComboBox.SelectedIndex = -1;
        }
    }
}
