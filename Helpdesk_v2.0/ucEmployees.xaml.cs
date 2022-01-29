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

namespace Helpdesk_v2._0
{
    /// <summary>
    /// Interaction logic for ucEmployees.xaml
    /// </summary>
    public partial class ucEmployees : UserControl
    {

        public ucEmployees()
        {
            InitializeComponent();

        }

        public class MyItem
        {
            public string id { get; set; }
            public string Name { get; set; }
            public string Gender { get; set; }
            public string Login_naam { get; set; }
            public string EmailAddress { get; set; }
            public string JobTitle { get; set; }
            public string PhoneNumber { get; set; }
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtSearch.Focus();

            string Q = "select top 100 p.BusinessEntityID, p.FirstName, p.MiddleName, p.LastName, pe.EmailAddress, e.LoginID, e.JobTitle, e.Gender, pp.PhoneNumber from Person.Person p inner join Person.EmailAddress pe on p.BusinessEntityID = pe.BusinessEntityID inner join HumanResources.Employee e on e.BusinessEntityID = p.BusinessEntityID left join Person.PersonPhone pp on p.BusinessEntityID = pp.BusinessEntityID Order by FirstName, LastName asc";
            LoadDataGrid(Q);
        }

        // When you press the Search button
        private void btnZoek_Click(object sender, RoutedEventArgs e)
        {
            string Q = "select top 100 p.BusinessEntityID, p.FirstName, p.MiddleName, p.LastName, pe.EmailAddress, e.LoginID, e.JobTitle, e.Gender, pp.PhoneNumber from Person.Person p inner join Person.EmailAddress pe on p.BusinessEntityID = pe.BusinessEntityID inner join HumanResources.Employee e on e.BusinessEntityID = p.BusinessEntityID left join Person.PersonPhone pp on p.BusinessEntityID = pp.BusinessEntityID WHERE FirstName LIKE '%' + @FirstName + '%' Order by FirstName, LastName asc";
            LoadDataGrid(Q);
        }

        private void LoadDataGrid(string Q)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
                {
                    using (SqlCommand CMD = new SqlCommand(Q, CN))
                    {
                        // IF NOTHING IS FILLED IN, FIRSTNAME WON'T GET PASSED
                        if (txtSearch.Text != null || txtSearch.Text != string.Empty)
                        {
                            CMD.Parameters.AddWithValue("@FirstName", txtSearch.Text);
                        }

                        CN.Open();
                        using (SqlDataReader DR = CMD.ExecuteReader())
                        {
                            // Put the data into the DataGrid
                            FillDataGrid(DR);
                        }
                        CN.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Cursor = null;
            }
        }

        public void FillDataGrid(SqlDataReader Reader)
        {
            // Clear the DataGrid
            dgResults.Items.Clear();

            while (Reader.Read())
            {
                string FullName = string.Empty;
                string id, Gender, Mail, Login_naam, JobTitle, PhoneNumber = string.Empty;

                // FullName
                id = Reader["BusinessEntityID"].ToString();
                FullName += Reader["FirstName"].ToString() + " ";
                if (Reader["MiddleName"].ToString() != "")
                {
                    FullName += Reader["MiddleName"].ToString() + " ";
                }
                FullName += Reader["LastName"].ToString();

                Gender = Reader["Gender"].ToString();
                JobTitle = Reader["JobTitle"].ToString();
                Login_naam = Reader["LoginID"].ToString();
                Mail = Reader["EmailAddress"].ToString();
                PhoneNumber = Reader["PhoneNumber"].ToString();

                // Add to the DataGrid
                dgResults.Items.Add(new MyItem { id = id, Name = FullName, Gender = Gender, JobTitle = JobTitle, Login_naam = Login_naam, EmailAddress = Mail, PhoneNumber = PhoneNumber });
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var row = e.Source as DataGridRow;

                DetailsWindow detailsWindow = new DetailsWindow(row.Item);

                detailsWindow.Show();
            }
        }
    }
}
