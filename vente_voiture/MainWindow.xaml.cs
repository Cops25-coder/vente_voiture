using System;
using System.Collections.ObjectModel;
using System.Windows;
using MySql.Data.MySqlClient;

namespace CarGestion
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Voiture> _voitures = new ObservableCollection<Voiture>();
        private const string ConnectionString = "Server=localhost;Port=3306;Database=car_gestion;Uid=root;Pwd=;";

        public MainWindow()
        {
            InitializeComponent();
            ChargerVoitures();
            VoituresGrid.ItemsSource = _voitures;
        }

        private void ChargerVoitures()
        {
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT id, marque, modele, annee FROM voitures", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _voitures.Add(new Voiture
                            {
                                Id = reader.GetInt32("id"),
                                Marque = reader.IsDBNull(reader.GetOrdinal("marque")) ? null : reader.GetString("marque"),
                                Modele = reader.IsDBNull(reader.GetOrdinal("modele")) ? null : reader.GetString("modele"),
                                Annee = reader.GetInt32("annee")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de chargement : {ex.Message}");
            }
        }

        private void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            var fenetre = new VoitureWindow();
            if (fenetre.ShowDialog() == true)
            {
                var nouvelleVoiture = fenetre.Voiture;
                nouvelleVoiture.Id = _voitures.Count > 0 ? _voitures.Max(v => v.Id) + 1 : 1;
                _voitures.Add(nouvelleVoiture);
                SauvegarderVoiture(nouvelleVoiture);
            }
        }

        private void Modifier_Click(object sender, RoutedEventArgs e)
        {
            if (VoituresGrid.SelectedItem is Voiture voiture)
            {
                var fenetre = new VoitureWindow(voiture);
                if (fenetre.ShowDialog() == true)
                {
                    SauvegarderVoiture(voiture);
                    VoituresGrid.Items.Refresh();
                }
            }
            else
                MessageBox.Show("Sélectionnez une voiture à modifier.");
        }

        private void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            if (VoituresGrid.SelectedItem is Voiture voiture)
            {
                if (MessageBox.Show($"Supprimer {voiture.Marque} {voiture.Modele} ?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _voitures.Remove(voiture);
                    SupprimerVoiture(voiture.Id);
                }
            }
            else
                MessageBox.Show("Sélectionnez une voiture à supprimer.");
        }

        private void Rafraichir_Click(object sender, RoutedEventArgs e)
        {
            _voitures.Clear();
            ChargerVoitures();
            VoituresGrid.Items.Refresh();
        }

        private void SauvegarderVoiture(Voiture voiture)
        {
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        "INSERT INTO voitures (id, marque, modele, annee) VALUES (@id, @marque, @modele, @annee) " +
                        "ON DUPLICATE KEY UPDATE marque = @marque, modele = @modele, annee = @annee", conn);
                    cmd.Parameters.AddWithValue("@id", voiture.Id);
                    cmd.Parameters.AddWithValue("@marque", voiture.Marque ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@modele", voiture.Modele ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@annee", voiture.Annee);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de sauvegarde : {ex.Message}");
            }
        }

        private void SupprimerVoiture(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM voitures WHERE id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de suppression : {ex.Message}");
            }
        }
    }

    public class Voiture
    {
        public int Id { get; set; }
        public string? Marque { get; set; }
        public string? Modele { get; set; }
        public int Annee { get; set; }
    }
}