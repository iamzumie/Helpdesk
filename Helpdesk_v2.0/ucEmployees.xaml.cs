using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Data;

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
            public string BindingId { get; set; }
            public string BindingFirstName { get; set; }
            public string BindingMiddleName { get; set; }
            public string BindingLastName { get; set; }
            public string BindingName { get; set; }
            public string BindingGender { get; set; }
            public string BindingLogin_naam { get; set; }
            public string BindingEmailAddress { get; set; }
            public string BindingJobTitle { get; set; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtSearch.Focus();
            LoadDataGrid();
        }

        // When you press the Search button
        private void btnZoek_Click(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
        }

        public void LoadDataGrid()
        {
            try
            {
                this.Cursor = Cursors.Wait;
                using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
                {
                    using (SqlCommand CMD = new SqlCommand(Properties.Resources.S_Employees, CN))
                    {
                        CMD.CommandType = CommandType.StoredProcedure;
                        CMD.Parameters.AddWithValue("@FirstName", txtSearch.Text); // default value is empty anyway

                        CN.Open();
                        using (SqlDataReader DR = CMD.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            FillDataGrid(DR);
                        }
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
                string firstName, middleName, lastName, fullName = string.Empty;
                string id, gender, mail, loginName, jobTitle = string.Empty;

                // FullName
                id = Reader["BusinessEntityID"].ToString();
                firstName = Reader["FirstName"].ToString();
                middleName = Reader["MiddleName"].ToString();
                lastName = Reader["LastName"].ToString();

                fullName += firstName + " ";
                if (middleName != "")
                {
                    fullName += middleName + " ";
                }
                fullName += lastName;

                gender = Reader["Gender"].ToString();
                jobTitle = Reader["JobTitle"].ToString();
                loginName = Reader["LoginID"].ToString();
                mail = Reader["EmailAddress"].ToString();

                // Add to the DataGrid
                dgResults.Items.Add(new MyItem { BindingId = id, BindingName = fullName, BindingGender = gender, BindingJobTitle = jobTitle, BindingLogin_naam = loginName, BindingEmailAddress = mail });
            }
        }
        #endregion

        #region EVENTS
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                // Get the SelectedItem
                MyItem classObj = dgResults.SelectedItem as MyItem;

                // Push the BindingId to the new Window
                DetailsWindow detailWindow = new DetailsWindow(Convert.ToInt32(classObj.BindingId));
                detailWindow.ShowDialog();

                // When Employee page gets updated, it will reload the DataGrid
                if (detailWindow.DialogResult == true)
                {
                    LoadDataGrid();
                }
            }
        }
      
        #endregion
    }
}
