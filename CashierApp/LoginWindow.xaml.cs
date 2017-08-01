using System;
using System.Data;
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
using System.Data.SqlClient;

namespace CashierApp
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private string currentDirectory = "a";
        private string con;

        public LoginWindow()
        {
            InitializeComponent();
            GetDirectory();
            
        }

        

        void GetDirectory()
        {
            currentDirectory = System.IO.Directory.GetParent
                           (System.IO.Directory.GetParent
                           (System.IO.Directory.GetParent
                           (System.AppDomain.CurrentDomain.BaseDirectory.ToString()
                           ).ToString()).ToString().ToString()).ToString();
            con = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + currentDirectory + @"\LocalPerpus.mdf;Integrated Security=True";

        }

        private void usernameText_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(usernameText.Text)) {
                usernameHint.Visibility = Visibility.Visible;
                usernameText.Visibility = Visibility.Collapsed;
            }
        }

        private void usernameHint_GotFocus(object sender, RoutedEventArgs e)
        {
            usernameHint.Visibility = Visibility.Collapsed;
            usernameText.Visibility = Visibility.Visible;
            usernameText.Focus();
        }

        private void passwordHint_GotFocus(object sender, RoutedEventArgs e)
        {
            passwordHint.Visibility = Visibility.Collapsed;
            passwordText.Visibility = Visibility.Visible;
            passwordText.Focus();
        }

        private void passwordText_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(passwordText.Password)) {
                passwordHint.Visibility = Visibility.Visible;
                passwordText.Visibility = Visibility.Collapsed;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            using (SqlConnection connection = new SqlConnection()) {
                connection.ConnectionString = con;
                connection.Open();
                SqlDataAdapter data = new SqlDataAdapter("SELECT * FROM userData  where username = '" + usernameText.Text.Trim() + "' AND password ='" + passwordText.Password + "'", connection);
                DataTable table = new DataTable();
                data.Fill(table);
                if(table.Rows.Count == 1){
                    MainWindow main = new MainWindow();
                    main.Show();
                    this.Close();
                }
                
            }
        }

        private void TextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Settings setting = new Settings();
            MessageBox.Show(usingLocal.ToString());
            setting.Show();
        }
    }
}
