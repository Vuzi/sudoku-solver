using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace SudokuSolver
{
    public class SudokuViewModel
    {
        public string NomApplication { get; set; }

        public ObservableCollection<Grille> GrilleList { get; set; }
        public Grille GrilleSelect { get; set; }

        public SudokuViewModel()
        {
            NomApplication = "Super Application de Sudoku !";
            GrilleList = new ObservableCollection<Grille>();
            GrilleList.Add(new Grille { Nom = "Grille 1", Date = "16/06/2015 19:34:00", Symboles = "123456789" });
            GrilleList.Add(new Grille { Nom = "Grille 2", Date = "16/06/2015 19:34:00", Symboles = "123456789" });
            GrilleList.Add(new Grille { Nom = "Grille 3", Date = "16/06/2015 19:34:00", Symboles = "123456789" });
            foreach (var grille in GrilleList)
            {
                grille.InitTabCases();
            }
        }

        public void AjouterGrille()
        {
            int numGrille = GrilleList.Count +1;
            Grille uneGrille = new Grille{ Nom = "Grille "+ numGrille.ToString(), Date = DateTime.Now.ToString(), Symboles = "123456789" };
            uneGrille.InitTabCases();
            GrilleList.Add(uneGrille);
        }

        public void SupprimerGrille()
        {
            if (GrilleSelect == null)
                System.Windows.MessageBox.Show("Aucun item sélectionné !", "Info");
            GrilleList.Remove(GrilleSelect);
        }
    }
}
