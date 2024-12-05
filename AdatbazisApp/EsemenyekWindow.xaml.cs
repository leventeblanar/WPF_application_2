using System;
using System.Data;
using System.Windows;
using Microsoft.Data.Sqlite;

namespace YourNamespace
{
    public partial class EsemenyekWindow : Window
    {
        private const string ConnectionString = "Data Source=doggo_manager.db";
        private DataRowView? _selectedRow;

        public EsemenyekWindow()
        {
            InitializeComponent();
            LoadEsemenyekData();
        }

        // Adatok betöltése
        private void LoadEsemenyekData()
        {
            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            Esemenyek.ID, 
                            Kutyak.Nev AS KutyaID, 
                            Esemenyek.GazdaID, 
                            Esemenyek.EsemenyLeirasa,
                            Esemenyek.Datum
                        FROM Esemenyek
                        LEFT JOIN Kutyak ON Esemenyek.KutyaID = Kutyak.ID";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            var dataTable = new DataTable();
                            dataTable.Load(reader);
                            EsemenyekDataGrid.ItemsSource = dataTable.DefaultView;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba az adatok betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Kutyák betöltése ComboBox-ba
        private void LoadKutyakToComboBox()
        {
            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT ID, Nev FROM Kutyak ORDER BY Nev ASC";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            KutyaComboBox.Items.Clear();

                            while (reader.Read())
                            {
                                var kutyaId = reader["ID"].ToString();
                                var kutyaNev = reader["Nev"].ToString();

                                KutyaComboBox.Items.Add($"{kutyaId} - {kutyaNev}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a kutyák betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Gomb események
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedRow = null;
            ShowForm();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (EsemenyekDataGrid.SelectedItem is DataRowView selectedRow)
            {
                _selectedRow = selectedRow;
                ShowForm();

                var kutyaNev = _selectedRow["Kutya"].ToString();
                foreach (var item in KutyaComboBox.Items)
                {
                    if (item.ToString()?.Contains($"- {kutyaNev}") == true)
                    {
                        KutyaComboBox.SelectedItem = item;
                        break;
                    }
                }

                EsemenyDatePicker.SelectedDate = _selectedRow["Dátum"] != DBNull.Value
                    ? DateTime.Parse(_selectedRow["Dátum"].ToString() ?? string.Empty)
                    : null;
                LeirasTextBox.Text = _selectedRow["Leírás"]?.ToString();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (EsemenyekDataGrid.SelectedItem is DataRowView selectedRow)
            {
                var result = MessageBox.Show("Biztosan törölni szeretnéd ezt a sort?", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var connection = new SqliteConnection(ConnectionString))
                        {
                            connection.Open();
                            string query = "DELETE FROM Esemenyek WHERE ID = @ID";

                            using (var command = new SqliteCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@ID", selectedRow["ID"]);
                                command.ExecuteNonQuery();
                                MessageBox.Show("A sor sikeresen törölve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadEsemenyekData();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Hiba a törlés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

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
                        query = "INSERT INTO Esemenyek (KutyaID, Dátum, Leírás) VALUES (@KutyaID, @Dátum, @Leírás)";
                    }
                    else
                    {
                        query = "UPDATE Esemenyek SET KutyaID = @KutyaID, Dátum = @Dátum, Leírás = @Leírás WHERE ID = @ID";
                    }

                    using (var command = new SqliteCommand(query, connection))
                    {
                        var selectedKutya = KutyaComboBox.SelectedItem?.ToString();
                        var kutyaId = selectedKutya?.Split('-')[0].Trim();
                        command.Parameters.AddWithValue("@KutyaID", string.IsNullOrEmpty(kutyaId) ? (object)DBNull.Value : kutyaId);

                        command.Parameters.AddWithValue("@Dátum", EsemenyDatePicker.SelectedDate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Leírás", string.IsNullOrEmpty(LeirasTextBox.Text) ? DBNull.Value : LeirasTextBox.Text);

                        if (_selectedRow != null)
                        {
                            command.Parameters.AddWithValue("@ID", _selectedRow["ID"]);
                        }

                        command.ExecuteNonQuery();
                        MessageBox.Show("Adatok sikeresen mentve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                        HideForm();
                        LoadEsemenyekData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba mentés közben: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowForm()
        {
            FormPanel.Visibility = Visibility.Visible;
            LoadKutyakToComboBox();
        }

        private void HideForm()
        {
            FormPanel.Visibility = Visibility.Collapsed;
            KutyaComboBox.SelectedIndex = -1;
            EsemenyDatePicker.SelectedDate = null;
            LeirasTextBox.Clear();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            HideForm();
        }
    }
}
