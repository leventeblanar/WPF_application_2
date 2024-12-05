using System;
using System.Data;
using System.Windows;
using Microsoft.Data.Sqlite;

namespace YourNamespace
{
    public partial class EgeszsegugyiNyilvantartasWindow : Window
    {
        private const string ConnectionString = "Data Source=doggo_manager.db";
        private DataRowView? _selectedRow;

        public EgeszsegugyiNyilvantartasWindow()
        {
            InitializeComponent();
            LoadEgeszsegugyiData();
        }

        private void LoadEgeszsegugyiData()
        {
            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            EgeszsegugyiNyilvantartas.ID, 
                            Kutyak.Nev AS KutyaID, 
                            EgeszsegugyiNyilvantartas.VizsgalatDatuma, 
                            EgeszsegugyiNyilvantartas.Megjegyzes
                        FROM EgeszsegugyiNyilvantartas
                        LEFT JOIN Kutyak ON EgeszsegugyiNyilvantartas.KutyaID = Kutyak.ID";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            var dataTable = new DataTable();
                            dataTable.Load(reader);
                            EgeszsegugyiDataGrid.ItemsSource = dataTable.DefaultView;
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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedRow = null;
            ShowForm();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (EgeszsegugyiDataGrid.SelectedItem is DataRowView selectedRow)
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

                // A dátum érték kezelése
                if (_selectedRow["VizsgalatDatuma"] != DBNull.Value)
                {
                    VizsgalatDatePicker.SelectedDate = DateTime.Parse(_selectedRow["VizsgalatDatuma"].ToString() ?? string.Empty);
                }
                else
                {
                    VizsgalatDatePicker.SelectedDate = null;
                }

                MegjegyzesTextBox.Text = _selectedRow["Megjegyzes"]?.ToString();
            }
            else
            {
                MessageBox.Show("Válassz ki egy sort a módosításhoz!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (EgeszsegugyiDataGrid.SelectedItem is DataRowView selectedRow)
            {
                var result = MessageBox.Show("Biztosan törölni szeretnéd ezt a sort?", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var connection = new SqliteConnection(ConnectionString))
                        {
                            connection.Open();
                            string query = "DELETE FROM EgeszsegugyiNyilvantartas WHERE ID = @ID";

                            using (var command = new SqliteCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@ID", selectedRow["ID"]);
                                command.ExecuteNonQuery();
                                MessageBox.Show("A sor sikeresen törölve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadEgeszsegugyiData();
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
                        query = "INSERT INTO EgeszsegugyiNyilvantartas (KutyaID, VizsgalatDatuma, Megjegyzes) VALUES (@KutyaID, @VizsgalatDatuma, @Megjegyzes)";
                    }
                    else
                    {
                        query = "UPDATE EgeszsegugyiNyilvantartas SET KutyaID = @KutyaID, VizsgalatDatuma = @VizsgalatDatuma, Megjegyzes = @Megjegyzes WHERE ID = @ID";
                    }

                    using (var command = new SqliteCommand(query, connection))
                    {
                        var selectedKutya = KutyaComboBox.SelectedItem?.ToString();
                        var kutyaId = selectedKutya?.Split('-')[0].Trim();
                        command.Parameters.AddWithValue("@KutyaID", string.IsNullOrEmpty(kutyaId) ? (object)DBNull.Value : kutyaId);

                        command.Parameters.AddWithValue("@VizsgalatDatuma", VizsgalatDatePicker.SelectedDate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Megjegyzes", string.IsNullOrEmpty(MegjegyzesTextBox.Text) ? DBNull.Value : MegjegyzesTextBox.Text);

                        if (_selectedRow != null)
                        {
                            command.Parameters.AddWithValue("@ID", _selectedRow["ID"]);
                        }

                        command.ExecuteNonQuery();
                        MessageBox.Show("Adatok sikeresen mentve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                        HideForm();
                        LoadEgeszsegugyiData();
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
            VizsgalatDatePicker.SelectedDate = null;
            MegjegyzesTextBox.Clear();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            HideForm();
        }
    }
}
