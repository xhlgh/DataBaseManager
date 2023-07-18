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

        //Przyciski odpowiadające za przełączanie się między danymi
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            context.Klienci.Load();

            klienciViewSource.Source = context.Klienci.Local;
        }

        private void LastCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            klienciViewSource.View.MoveCurrentToLast();
        }

        private void PreviousCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            klienciViewSource.View.MoveCurrentToPrevious();
        }

        private void NextCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            klienciViewSource.View.MoveCurrentToNext();
        }

        private void FirstCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            klienciViewSource.View.MoveCurrentToFirst();
        }

        //przycisk odpowiadający za usunięcie klienta
        private void DeleteCustomerCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            var cur = klienciViewSource.View.CurrentItem as Klienci;

            var cust = (from c in context.Klienci
                        where c.id_klienta == cur.id_klienta
                        select c).FirstOrDefault();

            if (cust != null)
            {
                foreach (var ord in cust.Transakcje.ToList())
                {
                    Delete_Order(ord);
                }
                context.Klienci.Remove(cust);
            }
            context.SaveChanges();
            klienciViewSource.View.Refresh();
        }
        
        //przyciski odpowiadające za dodawamie klientów i transakcji
        private void UpdateCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (klienciNewGrid.IsVisible)
            {
                Klienci newCustomer = new Klienci()
                {
                    id_klienta = Int32.Parse(add_id_klientaTextBox.Text),
                    imie = add_imieTextBox.Text,
                    nazwisko = add_nazwiskoTextBox.Text,
                    telefon = add_telefonTextBox.Text,
                };

                    if (newCustomer.telefon.Length == 9 || newCustomer.imie != null || newCustomer.nazwisko != null)
                    {
                        int len = context.Klienci.Local.Count();
                        int pos = len;
                        for (int i = 0; i < len; ++i)
                        {
                            if (context.Klienci.Local[i].id_klienta < 0 && newCustomer.id_klienta < 0)
                            {
                                pos = i;
                                break;
                            }
                        }
                        context.Klienci.Local.Insert(pos, newCustomer);
                        klienciViewSource.View.Refresh();
                        klienciViewSource.View.MoveCurrentTo(newCustomer);
                    }
                    else
                    {
                        MessageBox.Show("Wprowadzono niepoprawne dane");
                    }

                klienciNewGrid.Visibility = Visibility.Collapsed;
                klienciGrid.Visibility = Visibility.Visible;
            }
            else if (transakcjeNewGrid.IsVisible)
            {

                Klienci currentCustomer = (Klienci)klienciViewSource.View.CurrentItem;

                Transakcje newOrder = new Transakcje()
                {
                    cena_brutto = Int32.Parse(add_cena_bruttoTextBox.Text),
                    data_transakcji = (DateTime)add_data_transakcjiDatePicker.SelectedDate,
                    id_klienta = currentCustomer.id_klienta,
                    id_pracownika = Int32.Parse(add_cena_bruttoTextBox.Text),
                    id_produktu = Int32.Parse(add_id_produktuTextBox.Text),
                    id_transakcji = Int32.Parse(add_id_transakcjiTextBox.Text),
                    ilosc = Int32.Parse(add_iloscTextBox.Text),
                };

                try
                {
                    newOrder.id_pracownika = Int32.Parse(add_id_pracownikaTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("ID pracownika musi być liczbą całkowitą.");
                    return;
                }

                try
                {
                    newOrder.id_produktu = Int32.Parse(add_id_produktuTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("ID produktu musi być liczbą całkowitą.");
                    return;
                }

                try
                {
                    newOrder.id_transakcji = Int32.Parse(add_id_transakcjiTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("ID transakcji musi być liczbą całkowitą.");
                    return;
                }

                try
                {
                    newOrder.cena_brutto = Convert.ToDecimal(add_cena_bruttoTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("Cena musi się konwertować na liczbę dziesiętną.");
                    return;
                }

                try
                {
                    newOrder.ilosc = Int32.Parse(add_iloscTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("Ilość musi być liczbą całkowitą.");
                    return;
                }

                context.Transakcje.Add(newOrder);
                transakcjeViewSource.View.Refresh();
            }

            context.SaveChanges();
        }

        private void AddCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            klienciGrid.Visibility = Visibility.Collapsed;
            transakcjeNewGrid.Visibility = Visibility.Collapsed;
            klienciNewGrid.Visibility = Visibility.Visible;


            foreach (var child in klienciNewGrid.Children)
            {
                var tb = child as TextBox;
                if (tb != null)
                {
                    tb.Text = "";
                }
            }
        }

        private void NewOrder_click(object sender, RoutedEventArgs e)
        {
            var cust = klienciViewSource.View.CurrentItem as Klienci;
            if (cust == null)
            {
                MessageBox.Show("Nie wybrano klienta.");
                return;
            }

            klienciGrid.Visibility = Visibility.Collapsed;
            klienciNewGrid.Visibility = Visibility.Collapsed;
            transakcjeNewGrid.UpdateLayout();
            transakcjeNewGrid.Visibility = Visibility.Visible;
        }

        //anulowanie
        private void CancelCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            add_cena_bruttoTextBox.Text = "";
            add_id_klientaTextBox.Text = "";
            add_id_pracownikaTextBox.Text = "";
            add_id_produktuTextBox.Text = "";
            add_id_transakcjiTextBox.Text = "";
            add_iloscTextBox.Text = "";

            klienciGrid.Visibility = Visibility.Visible;
            klienciNewGrid.Visibility = Visibility.Collapsed;
            transakcjeNewGrid.Visibility = Visibility.Collapsed;
        }
        //usuwanie
        private void Delete_Order(Transakcje order)
        {
            var ord = (from o in context.Transakcje.Local
                       where o.id_transakcji == order.id_transakcji
                       select o).FirstOrDefault();

            context.Transakcje.Remove(ord);
            context.SaveChanges();

            transakcjeViewSource.View.Refresh();
        }

        private void DeleteOrderCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Transakcje obj = e.Parameter as Transakcje;
            Delete_Order(obj);
        }
    }
}
