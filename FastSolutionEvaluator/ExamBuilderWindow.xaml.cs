using FastSolutionEvaluator.ExamBuilderClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FastSolutionEvaluator
{
    /// <summary>
    /// Interaction logic for ExamBuilderWindow.xaml
    /// </summary>
    public partial class ExamBuilderWindow : Window
    {
        public ExamBuilderClasses.Examen HuidigeExamen { get; set; } = new Examen();
        public ExamBuilderWindow()
        {
            InitializeComponent();
            this.DataContext = HuidigeExamen;
        }

        private void btnOpenExamen_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "XML Files|*.XML";
            if (dlg.ShowDialog().Value == true)
            {
                try
                {
                    HuidigeExamen = XMLHelper.LoadFromXml<Examen>(dlg.FileName);
                    lbVragen.ItemsSource = HuidigeExamen.Vragen;
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnSaveExamen_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "XML Files|*.XML";
            if (dlg.ShowDialog().Value == true)
            {
                XMLHelper.SaveToXml<Examen>(HuidigeExamen, dlg.FileName);
            }
        }

        private void btnClearAll_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bent u zeker?Alle niet bewaarde zaken gaan verloren", "Opgelet", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                HuidigeExamen = new Examen();
                lbVragen.ItemsSource = HuidigeExamen.Vragen;
            }
        }

 

        private void btnVerwijderVraag_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bent u zeker dat u deze vraag wil verwijderen?", "Opgelet", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
                HuidigeExamen.Vragen.Remove((sender as FrameworkElement).DataContext as Vraag);

        }

        private void btnNieuweVraag_Click(object sender, RoutedEventArgs e)
        {
            HuidigeExamen.Vragen.Add(new Vraag());
            
        }

        private void cmbTypeVraag_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cmbTypeVraag.SelectedIndex>-1 && lbVragen.SelectedIndex>-1)
            (cmbTypeVraag.DataContext as Vraag).VraagType = (VraagType)cmbTypeVraag.SelectedIndex;
        }

        private void lbVragen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbTypeVraag.SelectedIndex = (int)(cmbTypeVraag.DataContext as Vraag).VraagType;
        }
    }
}
