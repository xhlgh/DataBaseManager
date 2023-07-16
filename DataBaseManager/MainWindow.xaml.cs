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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;

namespace DataBaseManager
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Sklep_papierniczyEntities context = new Sklep_papierniczyEntities();
        CollectionViewSource klienciViewSource;
        CollectionViewSource transakcjeViewSource;
        public MainWindow()
        {
            InitializeComponent();
            klienciViewSource = ((CollectionViewSource)(FindResource("klienciViewSource")));
            transakcjeViewSource = ((CollectionViewSource)(FindResource("klienciTransakcjeViewSource")));
            DataContext = this; 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            context.Klienci.Load();

            klienciViewSource.Source = context.Klienci.Local;
        }
    }
}
