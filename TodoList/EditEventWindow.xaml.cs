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

namespace TodoList {
    /// <summary>
    /// Logika interakcji dla klasy EditEventWindow.xaml
    /// </summary>
    public partial class EditEventWindow : Window {
        private int selected_ID;

        public EditEventWindow(int selected_ID) {
            InitializeComponent();
            this.selected_ID = selected_ID;
            load_Event_Data();
            set_Button_Content();
        }

        private Event? get_Selected_Event() { // Zwrócenie wybranego eventu
            using (var db = new EventDataBaseContext()) {
                return db.Events.FirstOrDefault(e => e.ID == selected_ID);
            }
        }

        private void load_Event_Data() { // Załadowanie danych wydarzenia do formularza

            var ev = get_Selected_Event();
            if (ev != null) {
                tb_editEvent_title.Text = ev.Title;
                tb_editEvent_description.Text = ev.Description;
                dp_editEvent_selected_date.SelectedDate = ev.Date;
            } else {
                CustomMessageBox CMBox = new CustomMessageBox("Error", "Nie znaleziono wydarzenia.");
                CMBox.ShowDialog();
                this.Close();
            }
        }

        // Funkcja zapisująca zmiany w wydarzeniu
        private void bt_editEvent_edit_Click(object sender, RoutedEventArgs e) {

            var ev = get_Selected_Event();
            if (ev != null) {

                if (string.IsNullOrWhiteSpace(tb_editEvent_title.Text)) {
                    CustomMessageBox CMBox = new CustomMessageBox("Error", "Tytuł wydarzenia nie może być pusty.");
                    CMBox.ShowDialog();
                    return;
                } else {

                    ev.Title = tb_editEvent_title.Text;
                    ev.Description = tb_editEvent_description.Text;
                    ev.Date = dp_editEvent_selected_date.SelectedDate ?? ev.Date;

                    using (var db = new EventDataBaseContext()) {
                        db.Events.Update(ev);
                        db.SaveChanges();
                    }
                    this.Close();

                }
            } else {
                CustomMessageBox CMBox = new CustomMessageBox("Error", "Nie znaleziono wydarzenia.");
                CMBox.ShowDialog();
            }
        }

        // Funkcja usuwająca wydarzenie
        private void bt_editEvent_delete_Click(object sender, RoutedEventArgs e) {

            CustomMessageBox CustomMessageBox = new CustomMessageBox("Warning", "Czy na pewno chcesz usunąć to wydarzenie?");
            bool? response = CustomMessageBox.ShowDialog();

            if (CustomMessageBox.UserResponse == true) {
                var ev = get_Selected_Event();
                if (ev != null) {

                    using (var db = new EventDataBaseContext()) {
                        db.Events.Remove(ev);
                        db.SaveChanges();
                    }
                   
                    this.Close();
                } else {
                    CustomMessageBox CMBox = new CustomMessageBox("Error", "Nie znaleziono wydarzenia.");
                    CMBox.ShowDialog();
                }
            }
        }

        // Funkcja oznaczająca wydarzenie jako wykonane lub cofająca wykonanie
        private void bt_editEvent_done_Click(object sender, RoutedEventArgs e) {

            var ev = get_Selected_Event();
            if (ev != null) {

                ev.IsCompleted = !ev.IsCompleted;

                using (var db = new EventDataBaseContext()) {
                    db.Events.Update(ev);
                    db.SaveChanges();
                }
                this.Close();
            } else {
                CustomMessageBox CMBox = new CustomMessageBox("Error", "Nie znaleziono wydarzenia.");
                CMBox.ShowDialog();
            }
        }

        // Funkcja ustawiająca tekst przycisku w zależności od stanu wydarzenia
        private void set_Button_Content() {
            var ev = get_Selected_Event();
            if (ev != null) {
                if (ev.IsCompleted == false) {
                    bt_editEvent_done.Content = "Wykonano";
                } else {
                    bt_editEvent_done.Content = "Cofnij wykonanie";
                }
            }
        }

    }
}
