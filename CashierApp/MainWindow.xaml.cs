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
using System.Data;
using System.Data.SqlClient;

namespace CashierApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string con, currentDirectory;
        private Button[] buttonMenu;
        private string language = "english";
        private Thickness addButtonMargin;
        private int currentRow, selectedTab;
        private DataTable mainGrid;

        public MainWindow()
        {
            InitializeComponent();
            buttonMenu = new Button[] { transaction, priceList};
            Style s = new Style();
            s.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Collapsed));
            tabControl.ItemContainerStyle = s;
            addButtonMargin = addButton.Margin;

            currentDirectory = System.IO.Directory.GetParent
               (System.IO.Directory.GetParent
               (System.IO.Directory.GetParent
               (System.AppDomain.CurrentDomain.BaseDirectory.ToString()
               ).ToString()).ToString().ToString()).ToString();
            con = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + currentDirectory + @"\LocalPerpus.mdf;Integrated Security=True";

            if (language == "Indonesia") {
                transaction.Content = "Transaksi";
                menuTitle.Content = "Transaksi";
                priceList.Content = "Daftar Harga";
            }
        }

        void ChangeButtonColor(int index)
        {
            for (int i = 0; i < buttonMenu.Length; i++) {
                if (i == index)
                    buttonMenu[i].Style = (Style)Application.Current.Resources["Accept"];
                else
                    buttonMenu[i].Style = (Style)Application.Current.Resources["Idle"];
            }
        }

        private void transaction_Click(object sender, RoutedEventArgs e)
        {
            RefreshTransaction();
            selectedTab = 0;
            ChangeButtonColor(0);

            menuTitle.Content = "Transaction";
            

            searchBox.Visibility = Visibility.Collapsed;
            searchImg.Visibility = Visibility.Collapsed;
            amountMoneyText.Visibility = Visibility.Collapsed;

            total.Visibility = Visibility.Visible;
            totalText.Visibility = Visibility.Visible;
            change.Visibility = Visibility.Visible;
            amountMoney.Visibility = Visibility.Visible;
            changeMoney.Visibility = Visibility.Visible;
            quantity.Visibility = Visibility.Visible;
            quantityBox.Visibility = Visibility.Visible;

            addButton.Margin = addButtonMargin;

            transactionTab.IsSelected = true;
        }

        void RefreshTransaction()
        {
            if (language == "Indonesia")
                FillDataGrid("TempCalculation", "no as No, name AS Nama, price as Harga, quantity as Jumlah, total as Total");
            else
                FillDataGrid("TempCalculation", "no as No, name AS Name, price as Price, quantity as Quantity, total as Total");

        }

        private void priceList_Click(object sender, RoutedEventArgs e)
        {
            RefreshPriceList();
            selectedTab = 1;
            ChangeButtonColor(1);

            menuTitle.Content = "Price List";

            total.Visibility = Visibility.Collapsed;
            totalText.Visibility = Visibility.Collapsed;
            amountMoney.Visibility = Visibility.Collapsed;
            changeMoney.Visibility = Visibility.Collapsed;
            amountMoneyText.Visibility = Visibility.Collapsed;
            quantity.Visibility = Visibility.Collapsed;
            quantityBox.Visibility = Visibility.Collapsed;
            change.Visibility = Visibility.Collapsed;

            searchBox.Visibility = Visibility.Visible;
            searchImg.Visibility = Visibility.Visible;
            

            addButton.Margin = quantityBox.Margin;

            transactionTab.IsSelected = true;
        }

        private void FillDataGrid(string tableName, string column)
        {
            GridData.SelectedItems.Clear();
            string cmdString = "SELECT "+column+" FROM "+tableName;
            using (SqlConnection connection = new SqlConnection(con)) {
                SqlCommand cmd = new SqlCommand(cmdString, connection);
                SqlDataAdapter data = new SqlDataAdapter(cmd);
                mainGrid = new DataTable();
                data.Fill(mainGrid);
                GridData.ItemsSource = mainGrid.DefaultView;
            }

            RefreshElement();
            
        }

        private void GridData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if(GridData.Items.Count <= 0 || GridData.SelectedItem == null)
            return;

            currentRow = GridData.Items.IndexOf(GridData.CurrentItem);
            DataRowView row = (DataRowView)GridData.SelectedItems[0];
            nameBox.Text = row[1].ToString();
            priceBox.Text = row[2].ToString();
            if(selectedTab == 0)
                quantityBox.Text = row[3].ToString();
            CountTotal();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInteger()) {
                MessageBox.Show("Please check your input");
                return;
            }
            if (selectedTab == 1) {
                string cmdString = "INSERT INTO PriceList (name, price) VALUES('" + nameBox.Text + "', '" + priceBox.Text + "')";
                using (SqlConnection connection = new SqlConnection(con)) {
                    SqlCommand cmd = new SqlCommand(cmdString, connection);
                    cmd.CommandType = CommandType.Text;
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
                RefreshPriceList();
                return;
            }

            DataRow[] foundData = mainGrid.Select(mainGrid.Columns[1].ColumnName +" = '"+nameBox.Text+"'");
            DataRow dataRow = mainGrid.NewRow();
            dataRow[0] = mainGrid.Rows.Count + 1;
            dataRow[1] = nameBox.Text.Trim();
            dataRow[2] = priceBox.Text;
            dataRow[3] = quantityBox.Text;
            if (!CheckInteger()) {
                MessageBox.Show("Price and Quantity must use numeric input");
                return;
            }
            dataRow[4] = Convert.ToInt32(priceBox.Text) * Convert.ToInt32(quantityBox.Text);
            RefreshElement();
            if (foundData.Length == 0) {
                mainGrid.Rows.Add(dataRow);
            }
            CountTotal();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!CheckInteger()) {
                MessageBox.Show("Please check your input");
                return;
            }

            if (selectedTab == 0) {
                DataRow dataRow = mainGrid.NewRow();
                dataRow[0] = mainGrid.Rows.Count + 1;
                dataRow[1] = nameBox.Text.Trim();
                dataRow[2] = priceBox.Text;
                dataRow[3] = quantityBox.Text;
                dataRow[4] = Convert.ToInt32(priceBox.Text) * Convert.ToInt32(quantityBox.Text);
                CountTotal();
            }

            if (selectedTab == 1) {
                DataRowView row = (DataRowView)GridData.SelectedItem;
                string name = row.Row.ItemArray[1].ToString();
                string cmdString = "UPDATE PriceList SET name ='"+nameBox.Text.Trim()+"', price ='"+priceBox.Text.Trim()+"' WHERE name ='"+name+"'";
                using (SqlConnection connection = new SqlConnection(con)){
                    SqlCommand cmd = new SqlCommand(cmdString, connection);
                    cmd.CommandType = CommandType.Text;
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
                RefreshPriceList();
            }

            RefreshElement();
            
        }

        void CountTotal()
        {
            if (selectedTab != 0)
                return;

            int totalPrice = 0, amountValue = Convert.ToInt32(amountMoney.Text), changeValue = 0;
            for (int i = 0; i < mainGrid.Rows.Count; i++)
			{
			    totalPrice += Convert.ToInt32(mainGrid.Rows[i].Field<string>(4));
			}
            changeValue = amountValue - totalPrice;
            changeMoney.Content = "Rp. " + changeValue;
            total.Content = "Rp. "+ totalPrice;
        }

        private bool CheckInteger()
        {
            int x, y, z;
            return (Int32.TryParse(priceBox.Text, out x)) && (Int32.TryParse(quantityBox.Text, out y)) && (Int32.TryParse(amountMoney.Text, out z));
        }

        void RefreshElement()
        {
            for (int i = 0; i < mainGrid.Rows.Count; i++) {
                mainGrid.Rows[i].SetField(mainGrid.Columns[0], Convert.ToString(i+1));
            }
            GridData.Columns[0].Width = DataGridLength.SizeToHeader;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (selectedTab == 0) {
                mainGrid.Rows[currentRow].Delete();
                CountTotal();
            }
            if (selectedTab == 1) {
                string cmdString = "DELETE FROM PriceList WHERE name = '" + nameBox.Text + "'";
                using (SqlConnection connection = new SqlConnection(con)) {
                    SqlCommand cmd = new SqlCommand(cmdString, connection);
                    cmd.CommandType = CommandType.Text;
                    connection.Open();
                    cmd.ExecuteNonQuery();    
                }
                RefreshPriceList();
            }
            RefreshElement();
        }

        private void nameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            listName.Items.Clear();
            string cmdString = "SELECT * FROM PriceList WHERE name LIKE'" + nameBox.Text.Trim() + "%'";
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(con)) {
                SqlCommand cmd = new SqlCommand(cmdString, connection);
                SqlDataAdapter data = new SqlDataAdapter(cmd);
                data.Fill(table);
            }
            if (table.Rows.Count >= 1) {
                for (int i = 0; i < table.Rows.Count; i++) {
                    listName.Items.Add(table.Rows[i][1]);
                }
                listName.Visibility = Visibility.Visible;
                
            }
            else
                listName.Visibility = Visibility.Collapsed;
            
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            listName.Visibility = Visibility.Collapsed;
        }

        private void listName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listName.Visibility = Visibility.Collapsed;
            if (listName.SelectedItem == null)
                return;

            string a = "";
            a += listName.SelectedItem.ToString();

            string cmdString = "SELECT * FROM PriceList WHERE name LIKE'" + a + "%'";
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(con)) {
                SqlCommand cmd = new SqlCommand(cmdString, connection);
                SqlDataAdapter data = new SqlDataAdapter(cmd);
                data.Fill(table);
            }

            nameBox.Text = table.Rows[0][1].ToString();
            priceBox.Text = table.Rows[0][2].ToString();
            listName.Visibility = Visibility.Collapsed;
        }

        private void amountMoney_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!CheckInteger())
                return;
            CountTotal();
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string cmdString = "SELECT * FROM PriceList WHERE name LIKE'" + searchBox.Text + "%'";
            using (SqlConnection connection = new SqlConnection(con)) {
                SqlCommand cmd = new SqlCommand(cmdString, connection);
                SqlDataAdapter data = new SqlDataAdapter(cmd);
                mainGrid.Clear();
                data.Fill(mainGrid);
            }
            RefreshElement();
        }

        void RefreshPriceList()
        {
            if (language == "Indonesia")
                FillDataGrid("PriceList", "no as No, name AS Nama, price AS Harga");
            else
                FillDataGrid("PriceList", "no as No, name AS Name, price AS Price");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshTransaction();
        }

        private void history_Click(object sender, RoutedEventArgs e)
        {
            ChangeButtonColor(2);
        }
    }
}
