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
    /// Logika interakcji dla klasy CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window {

        public bool? UserResponse { get; private set; }

        public CustomMessageBox(string mbType, string message) {
            InitializeComponent();
            show_Message(mbType, message);
        }

        // Rodzaje CustomMessageBox: Error, Info, Warning
        private void show_Message(string mbType, string message) {
            if (mbType == "Error") {
                lb_messagebox_info.Content = message;
                custom_messagebox.Title = "Błąd";
                bt_messagebox_no.Visibility = Visibility.Hidden;
                bt_messagebox_yes.Content = "OK";
            } else if (mbType == "Info") {
                lb_messagebox_info.Content = message;
                custom_messagebox.Title = "Informacja";
                bt_messagebox_no.Visibility = Visibility.Hidden;
                bt_messagebox_yes.Content = "OK";
            } else if (mbType == "Warning") {
                lb_messagebox_info.Content = message;
                custom_messagebox.Title = "Ostrzeżenie";
                bt_messagebox_no.Content = "Nie";
                bt_messagebox_yes.Content = "Tak";
            }
        }

        private void bt_messagebox_yes_Click(object sender, RoutedEventArgs e) {
            UserResponse = true;
            this.DialogResult = true;
            this.Close();
        }

        private void bt_messagebox_no_Click(object sender, RoutedEventArgs e) {
            UserResponse = false;
            this.DialogResult = false;
            this.Close();
        }
    }
}
