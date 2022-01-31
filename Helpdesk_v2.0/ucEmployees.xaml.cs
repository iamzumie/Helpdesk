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
        #region METHODS
        public ucEmployees()
        {
            InitializeComponent();
        }

        public class MyItem
        {
            public string id { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string Name { get; set; }
            public string Gender { get; set; }
            public string Login_naam { get; set; }
            public string EmailAddress { get; set; }
            public string JobTitle { get; set; }
            public string PhoneNumber { get; set; }
            public string Department { get; set; }
            public string DepartmentID { get; set; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtSearch.Focus();

            string Q = "select top 100 p.BusinessEntityID, p.FirstName, p.MiddleName, p.LastName, pe.EmailAddress, e.LoginID, e.JobTitle, e.Gender, pp.PhoneNumber, eh.DepartmentID, d.Name as Department from Person.Person p inner join Person.EmailAddress pe on p.BusinessEntityID = pe.BusinessEntityID inner join HumanResources.Employee e on e.BusinessEntityID = p.BusinessEntityID left join Person.PersonPhone pp on p.BusinessEntityID = pp.BusinessEntityID left join HumanResources.EmployeeDepartmentHistory eh on p.BusinessEntityID = eh.BusinessEntityID And EndDate is null left join HumanResources.Department d on eh.DepartmentID = d.DepartmentID Order by FirstName, LastName asc";
            LoadDataGrid(Q);
        }

        // When you press the Search button
        private void btnZoek_Click(object sender, RoutedEventArgs e)
        {
            string Q = "select top 100 p.BusinessEntityID, p.FirstName, p.MiddleName, p.LastName, pe.EmailAddress, e.LoginID, e.JobTitle, e.Gender, pp.PhoneNumber, eh.DepartmentID, d.Name as Department from Person.Person p inner join Person.EmailAddress pe on p.BusinessEntityID = pe.BusinessEntityID inner join HumanResources.Employee e on e.BusinessEntityID = p.BusinessEntityID left join Person.PersonPhone pp on p.BusinessEntityID = pp.BusinessEntityID left join HumanResources.EmployeeDepartmentHistory eh on p.BusinessEntityID = eh.BusinessEntityID And EndDate is null left join HumanResources.Department d on eh.DepartmentID = d.DepartmentID WHERE FirstName LIKE '%' + @FirstName + '%' Order by FirstName, LastName asc";
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
                string FirstName, MiddleName, LastName, FullName = string.Empty;
                string DepartmentID, Department = string.Empty;
                string id, Gender, Mail, Login_naam, JobTitle, PhoneNumber = string.Empty;

                // FullName
                id = Reader["BusinessEntityID"].ToString();
                FirstName = Reader["FirstName"].ToString();
                MiddleName = Reader["MiddleName"].ToString();
                LastName = Reader["LastName"].ToString();
                DepartmentID = Reader["DepartmentID"].ToString();
                Department = Reader["Department"].ToString();

                FullName += FirstName + " ";
                if (MiddleName != "")
                {
                    FullName += MiddleName + " ";
                }
                FullName += LastName;

                Gender = Reader["Gender"].ToString();
                JobTitle = Reader["JobTitle"].ToString();
                Login_naam = Reader["LoginID"].ToString();
                Mail = Reader["EmailAddress"].ToString();
                PhoneNumber = Reader["PhoneNumber"].ToString();

                // Add to the DataGrid
                dgResults.Items.Add(new MyItem { id = id, FirstName = FirstName, MiddleName = MiddleName, LastName = LastName, Name = FullName, Department = Department, DepartmentID = DepartmentID, Gender = Gender, JobTitle = JobTitle, Login_naam = Login_naam, EmailAddress = Mail, PhoneNumber = PhoneNumber });
            }
        }
        #endregion

        #region EVENTS
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var row = e.Source as DataGridRow;

                DetailsWindow detailsWindow = new DetailsWindow(row.Item);

                detailsWindow.Show();
            }
        }
        #endregion
    }
}
