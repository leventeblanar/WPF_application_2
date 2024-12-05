using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace YourNamespace
{
    public class DatabaseHelper
    {
        private const string ConnectionString = "Data Source=doggo_manager.db";

        // Adatbázis kapcsolat tesztelése
        public static void TestConnection()
        {
            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine("Sikeres adatbázis-kapcsolat!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Adatbázis kapcsolat sikertelen: {ex.Message}");
                throw; // Csak akkor szükséges, ha a hívó hely kezeli a kivételt
            }
        }

        // Táblanevek lekérdezése
        public static List<string> GetTableNames()
        {
            var tableNames = new List<string>();

            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();

                    // Lekérdezés a táblák neveire
                    string query = "SELECT name FROM sqlite_master WHERE type='table' AND name != 'sqlite_sequence';";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tableNames.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba a táblanevek lekérdezésekor: {ex.Message}");
                throw; // Ha a hívó hely kezeli a kivételt
            }

            return tableNames;
        }

        // Általános adatbázis kapcsolat ellenőrzése (alternatíva a TestConnection metódushoz)
        public static void TestDatabaseConnection()
        {
            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine("Adatbázis kapcsolat sikeres!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Adatbázis hiba: {ex.Message}");
                throw;
            }
        }
    }
}
