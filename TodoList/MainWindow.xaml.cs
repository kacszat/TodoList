using System.ComponentModel;
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

namespace TodoList {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private DateTime? selectedDate; // Przechowywanie wybranej daty z kalendarza
        bool IsCurrentEventsShown = false;  // Flaga do sprawdzania czy są pokazywane najbliższe wydarzenia

        private GridViewColumnHeader? lastHeaderClicked = null; // Przechowywanie ostatnio klikniętego nagłówka kolumny z ListView
        private ListSortDirection lastSortDirection = ListSortDirection.Ascending; // Przechowywanie ostatniego kierunku sortowania

        public MainWindow() {
            InitializeComponent();
            show_Notification();
            lv_events.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(lv_events_Click)); // Dodanie obsługi kliknięcia nagłówka kolumny
            set_Current_Date();
            load_Events();
        }

        // Dzisiejsza data
        private void set_Current_Date() {
            lb_current_date.Content = DateTime.Now.ToString("dd.MM.yyyy");
            selectedDate = DateTime.Today;
            set_Date_Info();
        }

        // Ustawienie informacji o wybranej dacie
        private void set_Date_Info() {
            if (selectedDate.HasValue && !IsCurrentEventsShown) { //Jeśli wybrano datę (not null)
                lb_info.Content = $"Wydarzenia dla: {selectedDate.Value.ToString("dd.MM.yyyy")}";
            }
        }

        // Wybranie daty z kalendarza
        private void dp_selected_date_SelectedDateChanged(object sender, SelectionChangedEventArgs e) {

            selectedDate = dp_selected_date.SelectedDate;
            if (selectedDate.HasValue) { //Jeśli wybrano datę (not null)
                IsCurrentEventsShown = false;
                set_Date_Info();
                load_Events();
            }
        }

        // Dodanie nowego wydarzenia
        private void bt_create_events_Click(object sender, RoutedEventArgs e) {

            AddEventWindow addEventWindow = new AddEventWindow();
            bool? result = addEventWindow.ShowDialog(); // Oczekiwanie na zamknięcie okna

            if (addEventWindow.NewEventDate.HasValue) {
                selectedDate = addEventWindow.NewEventDate.Value.Date;
                dp_selected_date.SelectedDate = selectedDate;
            } else {
                selectedDate = DateTime.Now;
            }
            IsCurrentEventsShown = false;
            load_Events();
            set_Date_Info();
        }

        // Załadowanie wydarzeń z bazy danych
        public void load_Events() {

            using (var db = new EventDataBaseContext()) {
                db.Database.EnsureCreated();
                var events_list = new List<Event>();

                if (IsCurrentEventsShown) {
                    DateTime today = DateTime.Today;
                    DateTime endDate = today.AddDays(3);

                    lv_events.ItemsSource = db.Events.Where(ev => ev.Date.Date >= today && ev.Date.Date <= endDate).ToList();

                } else {
                    if (selectedDate != null) {
                        events_list = db.Events.Where(ev => ev.Date.Date == selectedDate).ToList();
                    } else {
                        events_list = db.Events.ToList();
                    }

                    lv_events.ItemsSource = events_list;
                }
            }

        }

        // Edytowanie wydarzenia
        private void lv_events_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (lv_events.SelectedItem is Event selectedEvent) { // Sprawdzenie czy wybrano wydarzenie
                EditEventWindow editEventWindow = new EditEventWindow(selectedEvent.ID);
                bool? result = editEventWindow.ShowDialog(); // Oczekiwanie na zamknięcie okna

                if (!selectedDate.HasValue && !IsCurrentEventsShown) {   // Jeśli nie wybrano daty i nie są pokazywane najbliższe, ustaw dzisiejszą
                    selectedDate = DateTime.Now;
                }
                load_Events();
                set_Date_Info();
            }
        }

        // Pokazanie najbliższych wydarzeń
        private void bt_current_events_Show(object sender, RoutedEventArgs e) {

            dp_selected_date.SelectedDate = DateTime.Today;
            IsCurrentEventsShown = true;
            load_Events();
            lb_info.Content = "Wydarzenia dla trzech najbliższych dni.";
        }

        // Sortowanie według nazw kolumn w ListView
        private void lv_events_Click(object sender, RoutedEventArgs e) {

            if (e.OriginalSource is GridViewColumnHeader headerClicked && headerClicked.Column != null) {
                string sortBy = (headerClicked.Column.DisplayMemberBinding as Binding)?.Path.Path ?? headerClicked.Column.Header.ToString(); // Pobranie nazwy właściwości do sortowania
                if (sortBy == "Data wydarzenia") {
                    sortBy = "Date";
                }

                ListSortDirection direction = ListSortDirection.Ascending;

                if (headerClicked == lastHeaderClicked) {
                    direction = lastSortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;  // Odwrócenie kierunku sortowania
                }

                just_Sort(sortBy, direction);

                lastHeaderClicked = headerClicked;
                lastSortDirection = direction;
            }
        }

        // Funkcja odpowiedzialna bezpośrednio za sortowanie w ListView
        private void just_Sort(string sortBy, ListSortDirection direction) {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(lv_events.ItemsSource);  // Pobranie widoku kolekcji
            dataView.SortDescriptions.Clear();
            dataView.SortDescriptions.Add(new SortDescription(sortBy, direction));
            dataView.Refresh();
        }

        // Pokazanie powiadomienia o wydarzeniach zapisanych na dziś
        private void show_Notification() {
            StringBuilder notificationMessage = new StringBuilder();
            notificationMessage.AppendLine("UWAGA!");
            notificationMessage.AppendLine("Masz dziś zapisane wydarzenia!");
            int count = count_Todays_Events();
            if (count > 0) {
                notificationMessage.AppendLine($"Liczba wydarzeń: {count}");
                MessageBox.Show(notificationMessage.ToString(), "Powiadomienie", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Funkcja zliczająca wydarzenia zapisane na dziś
        private int count_Todays_Events() {
            using (var db = new EventDataBaseContext()) {
                DateTime today = DateTime.Today;
                return db.Events.Count(ev => ev.Date.Date == today);
            }
        }

    }
}