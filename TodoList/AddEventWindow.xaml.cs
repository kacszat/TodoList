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

namespace TodoList {
    /// <summary>
    /// Logika interakcji dla klasy AddEventWindow.xaml
    /// </summary>
    public partial class AddEventWindow : Window {

        Event newEvent = new Event();
        public DateTime? NewEventDate { get; private set; } // Właściwość do przechowywania daty nowego wydarzenia

        public AddEventWindow() {
            InitializeComponent();
        }

        private void bt_addEvent_add_Click(object sender, RoutedEventArgs e) {

            // Powiązanie danych z formularza do zmiennych
            newEvent.Title = tb_addEvent_title.Text;
            newEvent.Description = tb_addEvent_description.Text;
            newEvent.Date = (DateTime)dp_addEvent_selected_date.SelectedDate;
            newEvent.IsCompleted = false;

            // Walidacja danych
            if (string.IsNullOrWhiteSpace(newEvent.Title)) {
                MessageBox.Show("Tytuł wydarzenia nie może być pusty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            } else if (newEvent.Date == null) {
                MessageBox.Show("Proszę wybrać datę wydarzenia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            } else {
                using (EventDataBaseContext db = new EventDataBaseContext()) {
                    db.Database.EnsureCreated(); // Upewnienie się, że baza danych jest utworzona, jeśli nie, utwórz ją
                    db.Events.Add(newEvent);
                    db.SaveChanges();
                }

                //MessageBox.Show("Wydarzenie zapisane!");
                NewEventDate = newEvent.Date;
                this.Close();
            }

        }
    }
}
