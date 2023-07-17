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

        private void DeleteCustomerCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            // If existing window is visible, delete the customer and all their orders.  
            // In a real application, you should add warnings and allow the user to cancel the operation.  
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

        // Commit changes from the new customer form, the new order form,  
        // or edits made to the existing customer form.  
        private void UpdateCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (klienciNewGrid.IsVisible)
            {
                // Create a new object because the old one  
                // is being tracked by EF now.  
                Klienci newCustomer = new Klienci()
                {
                    id_klienta = Int32.Parse(add_id_klientaTextBox.Text),
                    imie = add_imieTextBox.Text,
                    nazwisko = add_nazwiskoTextBox.Text,
                    telefon = add_telefonTextBox.Text,
                };

                // Perform very basic validation  
                    if (newCustomer.telefon.Length == 9 || newCustomer.imie != null || newCustomer.nazwisko != null)
                    {
                        // Insert the new customer at correct position:  
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
                // Order ID is auto-generated so we don't set it here.  
                // For CustomerID, address, etc we use the values from current customer.  
                // User can modify these in the datagrid after the order is entered.  

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

                // Add the order into the EF model  
                context.Transakcje.Add(newOrder);
                transakcjeViewSource.View.Refresh();
            }

            // Save the changes, either for a new customer, a new order  
            // or an edit to an existing customer or order.
            context.SaveChanges();
        }

        // Sets up the form so that user can enter data. Data is later  
        // saved when user clicks Commit.  
        private void AddCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            klienciGrid.Visibility = Visibility.Collapsed;
            transakcjeNewGrid.Visibility = Visibility.Collapsed;
            klienciNewGrid.Visibility = Visibility.Visible;

            // Clear all the text boxes before adding a new customer.  
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

        // Cancels any input into the new customer form  
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

        private void Delete_Order(Transakcje order)
        {
            // Find the order in the EF model.  
            var ord = (from o in context.Transakcje.Local
                       where o.id_transakcji == order.id_transakcji
                       select o).FirstOrDefault();

            // Delete all the order_details that have  
            // this Order as a foreign key  
            //foreach (var detail in ord.Order_Details.ToList())
            //{
            //    context.Order_Details.Remove(detail);
            //}

            // Now it's safe to delete the order.  
            context.Transakcje.Remove(ord);
            context.SaveChanges();

            // Update the data grid.  
            transakcjeViewSource.View.Refresh();
        }

        private void DeleteOrderCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            // Get the Order in the row in which the Delete button was clicked.  
            Transakcje obj = e.Parameter as Transakcje;
            Delete_Order(obj);
        }
    }
}
