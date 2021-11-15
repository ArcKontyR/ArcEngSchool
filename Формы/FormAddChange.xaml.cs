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
using System.Windows.Shapes;
using ArcEngSchool.Формы;
using Microsoft.Win32;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace ArcEngSchool.Формы
{
    /// <summary>
    /// Логика взаимодействия для formAddChange.xaml
    /// </summary>
    public partial class FormAddChange : Window
    {
        public string purpose;
        public ISShoolEntities db;
        public Client client;
        public FormAddChange()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Цель создания формы, в зависимости от неё меняются элементы управления и возможности.
            switch (purpose) 
            { 
                case "Add":
                    db.Client.Load();
                    lblFormPurpose.Content = lblFormPurpose.Content + " Добавление";
                    btnClientAdd.Visibility = Visibility.Visible;
                    btnClientPhotoAdd.Visibility = Visibility.Visible;
                    lblClientID.Visibility = Visibility.Hidden;
                    lblClientIDNum.Visibility = Visibility.Hidden;

                    btnTagAdd.Visibility = Visibility.Hidden;
                    btnTagAddNew.Visibility = Visibility.Hidden;
                    btnTagDelete.Visibility = Visibility.Hidden;

                    db.Gender.Load();
                    cbClientGender.ItemsSource = db.Gender.Local.Select(a => a.Code);

                    db.Tag.Load();
                    cbTags.ItemsSource = db.Tag.Local.Select(a => a.Title);

                    break;
                case "Change":
                    lblFormPurpose.Content = lblFormPurpose.Content + " Редактирование";
                    btnClientChange.Visibility = Visibility.Visible;
                    btnClientPhotoChange.Visibility = Visibility.Visible;

                    db.Gender.Load();
                    cbClientGender.ItemsSource = db.Gender.Local.Select(a => a.Code);

                    db.Tag.Load();
                    cbTags.ItemsSource = db.Tag.Local.Select(a=>a.Title);
                    
                    int i = 0;
                    foreach (Client p in db.Client.Include(p => p.Tag).Where(p=>p.ID == client.ID).ToList())
                    {
                        foreach (Tag a in p.Tag)
                        {
                                ListBoxItem lbItem = new ListBoxItem();
                                string colorStr = '#' + a.Color;
                                Color color = (Color)ColorConverter.ConvertFromString(colorStr);
                                SolidColorBrush brush = new SolidColorBrush(color);
                                lbItem.Foreground = brush;
                                lbItem.Content = a.Title;
                                lbTags.Items.Add(lbItem);
                            i += 1;
                        }
                    }


                    lblClientIDNum.Content = client.ID;
                    tbClientLastName.Text = client.LastName;
                    tbClientFirstName.Text = client.FirstName;
                    tbClientPatronymic.Text = client.Patronymic;
                    tbClientEmail.Text = client.Email;
                    tbClientPhone.Text = client.Phone;
                    dpClientBirthday.SelectedDate = client.Birthday;
                    cbClientGender.SelectedItem = client.GenderCode;

                    if (client.PhotoPath != null)
                    imgClientPhoto.Source = new BitmapImage(new Uri(client.PhotoPath, UriKind.RelativeOrAbsolute));
                    break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            FormMain main = Owner as FormMain;
            main.btnClientAdd.Visibility = Visibility.Visible;
            main.btnClientClear.Visibility = Visibility.Visible;
            main.btnClientChange.Visibility = Visibility.Visible;
            main.btnClientDelete.Visibility = Visibility.Visible;
            main.btnClientVisits.Visibility = Visibility.Visible;
            main.dgClients.ItemsSource = main.db.Client.Local;
            Close();
        }

        private void btnClientPhotoAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                MessageBox.Show(openFileDialog.FileName);

                imgClientPhoto.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void btnClientPhotoChange_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                MessageBox.Show(openFileDialog.FileName);

                imgClientPhoto.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void btnClientAdd_Click(object sender, RoutedEventArgs e)
        {
            Client clientAdd = new Client();
            clientAdd.FirstName = tbClientFirstName.Text;
            clientAdd.LastName = tbClientLastName.Text;
            clientAdd.Patronymic = tbClientPatronymic.Text;
            clientAdd.Phone = tbClientPhone.Text;
            clientAdd.Email = tbClientEmail.Text;
            clientAdd.Birthday = dpClientBirthday.SelectedDate.Value;
            clientAdd.RegistrationDate = DateTime.Now;
            clientAdd.GenderCode = cbClientGender.SelectedItem.ToString();
            if (imgClientPhoto.Source != null) clientAdd.PhotoPath = imgClientPhoto.Source.ToString();
            db.Client.Local.Add(clientAdd);
            db.SaveChanges();

            MessageBox.Show("Клиент добавлен");

            btnTagAdd.Visibility = Visibility.Visible;
            btnTagAddNew.Visibility = Visibility.Visible;
            btnTagDelete.Visibility = Visibility.Visible;

            btnClientAdd.IsEnabled = false;
        }

        private void btnClientChange_Click(object sender, RoutedEventArgs e)
        {
            FormMain main = Owner as FormMain;
            var temp = from cl in main.db.Client where cl.ID == client.ID select cl;
            foreach (var cl in temp)
            {
                cl.FirstName = tbClientFirstName.Text;
                cl.LastName = tbClientLastName.Text;
                cl.Patronymic = tbClientPatronymic.Text;
                cl.Phone = tbClientPhone.Text;
                cl.Email = tbClientEmail.Text;
                cl.Birthday = dpClientBirthday.SelectedDate.Value;
                cl.GenderCode = cbClientGender.SelectedItem.ToString();
                cl.PhotoPath = imgClientPhoto.Source.ToString();
            }
            db.SaveChanges();

            MessageBox.Show("Клиент изменен");
        }

        private void tbClientFIO_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //Проверка на буквы (с дополнительым знаком)
            if (e.Text != "-")
            {
                e.Handled = !char.IsLetter(e.Text, 0);
            }
            else e.Handled = false;
        }

        private void tbClientPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //Проверка на цифры (с дополнительыми знаками)
            if (e.Text != "-" && e.Text != "+" && e.Text != "(" && e.Text != ")")
            {
                e.Handled = !char.IsNumber(e.Text, 0);
            }
            else e.Handled = false;
        }

        private void lbTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbTags.SelectedIndex != -1)
            {
                btnTagDelete.IsEnabled = true;
            } else
            {
                btnTagDelete.IsEnabled = false;
            }
            
        }

        private void btnTagDelete_Click(object sender, RoutedEventArgs e)
        {
            //Удаление связи между клиентом и тэгом
            Client _client = db.Client.First(p => p.ID == client.ID);
            Tag _tag = _client.Tag.First(p => p.Title == ((ListBoxItem)lbTags.SelectedValue).Content.ToString());
            _tag.Client.Remove(_client);
            lbTags.Items.Clear();
            db.SaveChanges();

            MessageBox.Show("Тэг откреплен");

            //Чтение тэгов и перенос их в список ListBox
            int i = 0;
            foreach (Client p in db.Client.Include(p => p.Tag).Where(p => p.ID == client.ID).ToList())
            {
                foreach (Tag a in p.Tag)
                {
                    ListBoxItem lbItem = new ListBoxItem();
                    string colorStr = '#' + a.Color;
                    Color color = (Color)ColorConverter.ConvertFromString(colorStr);
                    SolidColorBrush brush = new SolidColorBrush(color);
                    lbItem.Foreground = brush;
                    lbItem.Content = a.Title;
                    lbTags.Items.Add(lbItem);
                    i += 1;
                }
            }
        }
               
        private void cpTagColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (cpTagColor.SelectedColor != null && tbTagNewTitle.Text !="")
            {
                btnTagAddNew.IsEnabled = true;
            }
            else
                btnTagAddNew.IsEnabled = false;
        }

        private void btnTagAddNew_Click(object sender, RoutedEventArgs e)
        {
            //Создание нового тэга
            Tag tag = new Tag();
            tag.Title = tbTagNewTitle.Text;
            tag.Color = cpTagColor.SelectedColor.ToString().Substring(3, 6);
            db.Tag.Local.Add(tag);
            db.SaveChanges();

            MessageBox.Show("Тэг добавлен");

            //Загрузка тэгов в ComboBox
            db.Tag.Load();
            cbTags.ItemsSource = db.Tag.Local;
        }

        private void btnTagAdd_Click(object sender, RoutedEventArgs e)
        {
            //В зависимости от цели, разная прогрузка тэгов
            if (purpose == "Change")
            {
                Client _client = db.Client.First(p => p.ID == client.ID);
                Tag _tag = db.Tag.First(p => p.Title == cbTags.SelectedValue.ToString());
                _tag.Client.Add(_client);
                lbTags.Items.Clear();
                db.SaveChanges();

                MessageBox.Show("Тэг прикреплен");
            }

            if (purpose == "Add")
            {
                db.Client.Load();
                int lastClientID = db.Client.Local.Last().ID;
                Client _client = db.Client.First(p => p.ID == lastClientID);
                Tag _tag = db.Tag.First(p => p.Title == cbTags.SelectedValue.ToString());
                _tag.Client.Add(_client);
                lbTags.Items.Clear();
                db.SaveChanges();

                MessageBox.Show("Тэг прикреплен");
            }

            int i = 0;
            int clientID = 0;

            if (purpose == "Change")
            {
                clientID = client.ID;
            } else
            {
                clientID = db.Client.Local.Last().ID;
            }    

            foreach (Client p in db.Client.Include(p => p.Tag).Where(p => p.ID == clientID).ToList())
            {
                foreach (Tag a in p.Tag)
                {
                    ListBoxItem lbItem = new ListBoxItem();
                    string colorStr = '#' + a.Color;
                    Color color = (Color)ColorConverter.ConvertFromString(colorStr);
                    SolidColorBrush brush = new SolidColorBrush(color);
                    lbItem.Foreground = brush;
                    lbItem.Content = a.Title;
                    lbTags.Items.Add(lbItem);
                    i += 1;
                }
            }
        }

        private void cbTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTags.SelectedIndex != -1)
            {
                btnTagAdd.IsEnabled = true;
            }
            else
            {
                btnTagAdd.IsEnabled = false;
            }
        }

        private void tbTagNewTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cpTagColor.SelectedColor != null && tbTagNewTitle.Text != "")
            {
                btnTagAddNew.IsEnabled = true;
            }
            else
                btnTagAddNew.IsEnabled = false;
        }

        private void tbClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbClientEmail.Text.Contains("@") && tbClientEmail.Text !="")
            {
                lblClientEmailChecking.Visibility = Visibility.Hidden;
                if (tbClientEmail.Text != "" && tbClientFirstName.Text != "" && tbClientLastName.Text != "" && tbClientPatronymic.Text != "" && tbClientPhone.Text != "" && cbClientGender.SelectedIndex != -1 && dpClientBirthday.SelectedDate != null)
                {
                    btnClientAdd.IsEnabled = true;
                }
                else
                {
                    btnClientAdd.IsEnabled = false;
                }
            } else
            {
                btnClientAdd.IsEnabled = false;
                lblClientEmailChecking.Visibility = Visibility.Visible;
            }
        }
            

        private void dpClientBirthday_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbClientEmail.Text != "" && tbClientFirstName.Text != "" && tbClientLastName.Text != "" && tbClientPatronymic.Text != "" && tbClientPhone.Text != "" && cbClientGender.SelectedIndex != -1 && dpClientBirthday.SelectedDate != null)
            {
                if (tbClientEmail.Text.Contains("@"))
                {
                    btnClientAdd.IsEnabled = true;
                    lblClientEmailChecking.Visibility = Visibility.Hidden;
                }
                else
                {
                    btnClientAdd.IsEnabled = false;
                    lblClientEmailChecking.Visibility = Visibility.Visible;
                }
            }
        }

        private void cbClientGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbClientEmail.Text != "" && tbClientFirstName.Text != "" && tbClientLastName.Text != "" && tbClientPatronymic.Text != "" && tbClientPhone.Text != "" && cbClientGender.SelectedIndex != -1 && dpClientBirthday.SelectedDate != null)
            {
                if (tbClientEmail.Text.Contains("@"))
                {
                    btnClientAdd.IsEnabled = true;
                    lblClientEmailChecking.Visibility = Visibility.Hidden;
                }
                else
                {
                    btnClientAdd.IsEnabled = false;
                    lblClientEmailChecking.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
