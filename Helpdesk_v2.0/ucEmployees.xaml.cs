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
            public string Name { get; set; }
            public string Gender { get; set; }
            public string Login_naam { get; set; }
            public string EmailAddress { get; set; }
            public string JobTitle { get; set; }
        }

        public void FillDataGrid(SqlDataReader Reader)
        {
            // Clear the DataGrid
            dgResults.Items.Clear();

            while (Reader.Read())
            {
                string FullName = string.Empty;
                string Gender, Mail, Login_naam, JobTitle = string.Empty;

                // FullName
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

                // Add to the DataGrid
                dgResults.Items.Add(new MyItem { Name = FullName, Gender = Gender, JobTitle = JobTitle, Login_naam = Login_naam, EmailAddress = Mail });
            }
        }

        // When you press the Search button
        private void btnZoek_Click(object sender, RoutedEventArgs e)
        {
            // String
            string Q = "select top 100 * from Person.Person p inner join Person.EmailAddress on p.BusinessEntityID = EmailAddress.BusinessEntityID inner join HumanResources.Employee e on e.BusinessEntityID = p.BusinessEntityID WHERE FirstName LIKE '%' + @FirstName + '%' Order by FirstName, LastName asc";

            try
            {
                using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
                {
                    using (SqlCommand CMD = new SqlCommand(Q, CN))
                    {
                        CMD.Parameters.AddWithValue("@FirstName", txtSearch.Text);

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
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtSearch.Focus();

            // Latest 100 made users
            string Q = "select top 100 * from Person.Person p inner join Person.EmailAddress on p.BusinessEntityID = EmailAddress.BusinessEntityID inner join HumanResources.Employee e on e.BusinessEntityID = p.BusinessEntityID Order by FirstName, LastName asc";

            try
            {
                using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
                {
                    using (SqlCommand CMD = new SqlCommand(Q, CN))
                    {
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
        }
    }
}
