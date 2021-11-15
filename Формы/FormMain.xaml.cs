using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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

namespace ArcEngSchool.Формы
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class FormMain : Window
    {
        public ISShoolEntities db = new ISShoolEntities();
        public FormMain()
        {
            InitializeComponent();
        }

        
        private void btnClientVisits_Click(object sender, RoutedEventArgs e)
        {
            FormClientVisits formClientVisits = new FormClientVisits();
            formClientVisits.Owner = this;
            formClientVisits.Show();
        }

        private void btnClientAdd_Click(object sender, RoutedEventArgs e)
        {
            FormAddChange formAddChange = new FormAddChange();
            formAddChange.Owner = this;
            formAddChange.purpose = "Add";
            formAddChange.db = db; 
            btnClientChange.Visibility = Visibility.Hidden;
            btnClientAdd.Visibility = Visibility.Hidden;
            btnClientDelete.Visibility = Visibility.Hidden;
            btnClientVisits.Visibility = Visibility.Hidden;
            formAddChange.Show();
        }

        private void btnClientChange_Click(object sender, RoutedEventArgs e)
        {
            FormAddChange formAddChange = new FormAddChange();
            formAddChange.Owner = this;
            formAddChange.purpose = "Change";
            formAddChange.db = db;
            Client client = (Client)dgClients.SelectedItem;
            formAddChange.client = db.Client.Where(a => a.ID == client.ID).ToArray()[0];
            btnClientChange.Visibility = Visibility.Hidden;
            btnClientAdd.Visibility = Visibility.Hidden;
            btnClientDelete.Visibility = Visibility.Hidden;
            btnClientVisits.Visibility = Visibility.Hidden;
            formAddChange.Show();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            db.Client.Load();
            dgClients.ItemsSource = db.Client.Local.OrderBy(z=> z.LastName);

        }

        private void dgClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgClients.SelectedIndex != -1)
            {
                btnClientDelete.IsEnabled = true;
                btnClientChange.IsEnabled = true;
                btnClientVisits.IsEnabled = true;
            }
            else
            {
                btnClientDelete.IsEnabled = false;
                btnClientChange.IsEnabled = false;
                btnClientVisits.IsEnabled = false;
            }
        }

        private void btnClientBirthdays_Click(object sender, RoutedEventArgs e)
        {
            dgClients.ItemsSource = db.Client.Local.Where(a => a.Birthday.Value.Month == DateTime.Now.Month);
            btnClientClear.Visibility = Visibility.Visible;
        }

        private void btnClientDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("Вы точно хотите удалить клиента? (Тэги будут откреплены)", "Удаление",MessageBoxButton.YesNo);
            if (dr == MessageBoxResult.Yes)
            {
                //удаление связей между клиентом и тэгами и последующее удаление клиента
                Client clientNeedsToDel = (Client)dgClients.SelectedItem;
                foreach (Tag a in clientNeedsToDel.Tag)
                {
                    a.Client.Remove(clientNeedsToDel);
                }
                db.Client.Remove(clientNeedsToDel);
                db.SaveChanges();
            }
        }

        private void btnClientClear_Click(object sender, RoutedEventArgs e)
        {
            dgClients.ItemsSource = db.Client.Local;
            btnClientClear.Visibility = Visibility.Hidden;
        }
    }
}
