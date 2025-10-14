using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TodoList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DateTime? selectedDate; // Przechowywanie wybranej daty z kalendarza

        public MainWindow()
        {
            InitializeComponent();
            set_Current_Date();
            load_Events();
        }

        // Dzisiejsza data
        private void set_Current_Date()
        {
            lb_current_date.Content = DateTime.Now.ToString("dd.MM.yyyy");
            selectedDate = DateTime.Now;
            set_Date_Info();
        }

        // Pokazanie najbliższych wydarzeń
        private void Button_Click_Current_Events(object sender, RoutedEventArgs e)
        {
            lb_info.Content = "Najbliższe wydarzenia";
        }

        // Ustawienie informacji o wybranej dacie
        private void set_Date_Info()
        {
            if (selectedDate.HasValue) //Jeśli wybrano datę (not null)
            {
                lb_info.Content = $"Wydarzenia dla: {selectedDate.Value.ToString("dd.MM.yyyy")}";
            }
        }

        // Wybranie daty z kalendarza
        private void dp_selected_date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedDate = dp_selected_date.SelectedDate;
            if (selectedDate.HasValue) //Jeśli wybrano datę (not null)
            {
                set_Date_Info();
                load_Events();
            }
        }

        // Dodanie nowego wydarzenia
        private void bt_create_events_Click(object sender, RoutedEventArgs e)
        {
            AddEventWindow addEventWindow = new AddEventWindow();
            bool? result = addEventWindow.ShowDialog(); // Oczekiwanie na zamknięcie okna

            if (!selectedDate.HasValue) {   // Jeśli nie wybrano daty, ustaw dzisiejszą
                selectedDate = DateTime.Now;
            }   
            load_Events();
            set_Date_Info();
        }

        // Załadowanie wydarzeń z bazy danych
        public void load_Events()
        {
            using (var db = new EventDataBaseContext())
            {
                db.Database.EnsureCreated();
                var events_list = new List<Event>();

                if (selectedDate != null) {
                    events_list = db.Events.Where(ev => ev.Date.Date == selectedDate).ToList();
                } else {
                    events_list = db.Events.ToList();
                }

                lv_events.ItemsSource = events_list;
            }
        }

        // Edytowanie wydarzenia
        private void lv_events_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_events.SelectedItem is Event selectedEvent)  // Sprawdzenie czy wybrano wydarzenie
            {
                EditEventWindow editEventWindow = new EditEventWindow(selectedEvent.ID);
                editEventWindow.ShowDialog();
            }
        }
    }
}