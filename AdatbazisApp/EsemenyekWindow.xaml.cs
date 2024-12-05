using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
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
                            Gazdak.Nev AS GazdaID,
                            Esemenyek.EsemenyLeirasa,
                            Esemenyek.Datum
                        FROM Esemenyek
                        LEFT JOIN Kutyak ON Esemenyek.KutyaID = Kutyak.ID
                        LEFT JOIN Gazdak ON Esemenyek.GazdaID = Gazdak.ID";

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

        private void KutyaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (KutyaComboBox.SelectedItem != null)
            {
                var selectedKutya = KutyaComboBox.SelectedItem.ToString();
                var kutyaId = selectedKutya?.Split('-')[0].Trim();

                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT Gazdak.ID, Gazdak.Nev FROM Kutyak JOIN Gazdak ON Kutyak.GazdaID = Gazdak.ID WHERE Kutyak.ID = @KutyaID";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@KutyaID", kutyaId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var gazdaId = reader["ID"].ToString();
                                var gazdaNev = reader["Nev"].ToString();
                                GazdaComboBox.Items.Clear();
                                GazdaComboBox.Items.Add($"{gazdaId} - {gazdaNev}");
                                GazdaComboBox.SelectedIndex = 0;
                            }
                        }
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
                        query = "INSERT INTO Esemenyek (KutyaID, GazdaID, Datum, EsemenyLeirasa) VALUES (@KutyaID, @GazdaID, @Datum, @EsemenyLeirasa)";
                    }
                    else
                    {
                        query = "UPDATE Esemenyek SET KutyaID = @KutyaID, GazdaID = @GazdaID, Datum = @Datum, EsemenyLeirasa = @EsemenyLeirasa WHERE ID = @ID";
                    }

                    using (var command = new SqliteCommand(query, connection))
                    {
                        var selectedKutya = KutyaComboBox.SelectedItem?.ToString();
                        var kutyaId = selectedKutya?.Split('-')[0].Trim();
                        command.Parameters.AddWithValue("@KutyaID", string.IsNullOrEmpty(kutyaId) ? (object)DBNull.Value : kutyaId);

                        var selectedGazda = GazdaComboBox.SelectedItem?.ToString();
                        var gazdaId = selectedGazda?.Split('-')[0].Trim();
                        command.Parameters.AddWithValue("@GazdaID", string.IsNullOrEmpty(gazdaId) ? (object)DBNull.Value : gazdaId);

                        command.Parameters.AddWithValue("@Datum", EsemenyDatePicker.SelectedDate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@EsemenyLeirasa", string.IsNullOrEmpty(LeirasTextBox.Text) ? DBNull.Value : LeirasTextBox.Text);

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
            GazdaComboBox.SelectedIndex = -1;
            EsemenyDatePicker.SelectedDate = null;
            LeirasTextBox.Clear();
        }

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

                var kutyaNev = _selectedRow["KutyaID"].ToString();
                foreach (var item in KutyaComboBox.Items)
                {
                    if (item.ToString()?.Contains($"- {kutyaNev}") == true)
                    {
                        KutyaComboBox.SelectedItem = item;
                        break;
                    }
                }

                var gazdaNev = _selectedRow["GazdaID"].ToString();
                foreach (var item in GazdaComboBox.Items)
                {
                    if (item.ToString()?.Contains($"- {gazdaNev}") == true)
                    {
                        GazdaComboBox.SelectedItem = item;
                        break;
                    }
                }

                EsemenyDatePicker.SelectedDate = _selectedRow["Datum"] != DBNull.Value
                    ? DateTime.Parse(_selectedRow["Datum"].ToString() ?? string.Empty)
                    : null;
                LeirasTextBox.Text = _selectedRow["EsemenyLeirasa"]?.ToString();
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            HideForm();
        }
    }
}
