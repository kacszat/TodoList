using Microsoft.Extensions.Logging;
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

namespace TodoList
{
    /// <summary>
    /// Logika interakcji dla klasy EditEventWindow.xaml
    /// </summary>
    public partial class EditEventWindow : Window
    {
        private int selected_ID;

        public EditEventWindow(int selected_ID) // Konstruktor przyjmujący ID wybranego wydarzenia
        {
            InitializeComponent();
            this.selected_ID = selected_ID;
            load_Event_Data();
        }

        private void load_Event_Data()  // Załadowanie danych wydarzenia do formularza
        {
            using (var db = new EventDataBaseContext())
            {
                var ev = db.Events.FirstOrDefault(e => e.ID == selected_ID);
                if (ev != null) {
                    tb_editEvent_title.Text = ev.Title;
                    tb_editEvent_description.Text = ev.Description;
                    dp_editEvent_selected_date.SelectedDate = ev.Date;
                } else {
                    MessageBox.Show("Nie znaleziono wydarzenia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
        }

        private void bt_editEvent_edit_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new EventDataBaseContext())
            {
                var ev = db.Events.FirstOrDefault(e => e.ID == selected_ID);
                if (ev != null)
                {
                    ev.Title = tb_editEvent_title.Text;
                    ev.Description = tb_editEvent_description.Text;
                    ev.Date = dp_editEvent_selected_date.SelectedDate ?? ev.Date;
                    db.SaveChanges();
                    MessageBox.Show("Wydarzenie zaktualizowane.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Nie znaleziono wydarzenia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            refresh_MainWindow();
        }

        private void bt_editEvent_delete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Czy na pewno chcesz usunąć to wydarzenie?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                using (var db = new EventDataBaseContext())
                {
                    var ev = db.Events.FirstOrDefault(e => e.ID == selected_ID);
                    if (ev != null)
                    {
                        db.Events.Remove(ev);
                        db.SaveChanges();
                        MessageBox.Show("Wydarzenie usunięte.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Nie znaleziono wydarzenia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            refresh_MainWindow();
        }

        private void bt_editEvent_done_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new EventDataBaseContext())
            {
                var ev = db.Events.FirstOrDefault(e => e.ID == selected_ID);
                if (ev != null)
                {
                    ev.IsCompleted = true;
                    db.SaveChanges();
                    MessageBox.Show("Wydarzenie wykonane!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Nie znaleziono wydarzenia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            refresh_MainWindow();
        }

        private void refresh_MainWindow()
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.load_Events();
            }
        }

    }
}
