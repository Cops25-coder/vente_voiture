using System.Windows;

namespace CarGestion
{
    public partial class VoitureWindow : Window
    {
        public Voiture Voiture { get; private set; }

        public VoitureWindow(Voiture? voiture = null)
        {
            InitializeComponent();
            Voiture = voiture ?? new Voiture { Marque = "", Modele = "", Annee = 0 };
            if (voiture != null)
            {
                MarqueTextBox.Text = voiture.Marque;
                ModeleTextBox.Text = voiture.Modele;
                AnneeTextBox.Text = voiture.Annee.ToString();
            }
        }

        private void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MarqueTextBox.Text) || string.IsNullOrWhiteSpace(ModeleTextBox.Text))
            {
                MessageBox.Show("Marque et Modèle sont requis.");
                return;
            }

            if (!int.TryParse(AnneeTextBox.Text, out int annee))
            {
                MessageBox.Show("L'année doit être un nombre valide.");
                return;
            }

            Voiture.Marque = MarqueTextBox.Text;
            Voiture.Modele = ModeleTextBox.Text;
            Voiture.Annee = annee;

            DialogResult = true;
            Close();
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}