using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace Helpdesk_v2._0
{
    /// <summary>
    /// Interaction logic for DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window
    {
        #region VARIABELEN
        DataTable DT;
        int PersonId = 0;
        bool Loading = true;
        #endregion


        #region METHODS
        public DetailsWindow(int ID)
        {
            InitializeComponent();

            // Getting ID from previous Window
            MyDetails idea = new MyDetails();
            idea.PersonId = ID;
            PersonId = idea.PersonId;

            Loading = true;
            LoadPage();
            FillDepartmentComboBox();
            FillGenderComboBox();
            Loading = false;
        }

        public class MyDetails
        {
            private int id2;

            public int PersonId {
                get
                {
                    return id2;
                }
                set
                {
                    id2 = value;
                }
            }
        }


        public void LoadPage()
        {
            try
            {
                this.Cursor = Cursors.Wait;
                using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
                {
                    using (SqlCommand CMD = new SqlCommand(Properties.Resources.S_Person, CN))
                    {
                        CMD.CommandType = CommandType.StoredProcedure;
                        CMD.Parameters.AddWithValue("@BusinessEntityID", PersonId);
                        
                        CN.Open();
                        using (SqlDataReader DR = CMD.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (DR.Read())
                            {
                                txtFirstName.Text = DR["FirstName"].ToString();
                                txtMiddleName.Text = DR["MiddleName"].ToString();
                                txtLastName.Text = DR["LastName"].ToString();
                                cboGender.SelectedValue = DR["Gender"].ToString();
                                txtJobTitle.Text = DR["JobTitle"].ToString();
                                txtLogin.Text = DR["LoginID"].ToString();
                                txtMail.Text = DR["EmailAddress"].ToString();
                                txtVestiging.SelectedValue = DR["DepartmentName"].ToString();
                            }
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

        private void FillDepartmentComboBox()
        {
            using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
            {
                using (SqlCommand CMD = new SqlCommand(Properties.Resources.S_SortVestiging, CN))
                {
                    CN.Open();
                    using (SqlDataReader DR = CMD.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (DR.Read())
                        {
                            txtVestiging.Items.Add(DR["Name"].ToString());
                        }
                    }
                }
            }
        }

        // Vul de combobox van de Gender aan met de keuzes, de binding zit in de XAML om de keuze te laten zien
        private void FillGenderComboBox()
        {
            cboGender.Items.Add("F");
            cboGender.Items.Add("M");
        }
        #endregion

        #region EVENTS
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
                {
                    using (SqlCommand CMD = new SqlCommand(Properties.Resources.U_Persoon, CN))
                    {
                        using (SqlDataAdapter DA = new SqlDataAdapter(CMD))
                        {
                            CMD.CommandType = CommandType.StoredProcedure;
                            CMD.Parameters.AddWithValue("@id", PersonId);
                            CMD.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                            CMD.Parameters.AddWithValue("@MiddleName", txtMiddleName.Text);
                            CMD.Parameters.AddWithValue("@LastName", txtLastName.Text);
                            CMD.Parameters.AddWithValue("@LoginID", txtLogin.Text);
                            CMD.Parameters.AddWithValue("@JobTitle", txtJobTitle.Text);
                            CMD.Parameters.AddWithValue("@Gender", cboGender.SelectedItem);
                            CMD.Parameters.AddWithValue("@EmailAddress", txtMail.Text);
                            CMD.Parameters.AddWithValue("@DepartmentName", txtVestiging.SelectedItem);
                            CMD.Parameters.AddWithValue("@DepartmentID", 0);
                            CMD.Parameters.AddWithValue("@ReturnValue", 0);
                            CMD.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;

                            DT = new DataTable();
                            DA.Fill(DT);

                            if ((int)CMD.Parameters["@ReturnValue"].Value == 999)
                            {
                                DialogResult = true;
                                this.Close();

                            }
                            else if ((int)CMD.Parameters["@ReturnValue"].Value == 998)
                            {
                                MessageBox.Show("The update has failed due to concurrency issues.");
                            }
                            else if ((int)CMD.Parameters["@ReturnValue"].Value == 997)
                            {
                                MessageBox.Show("The update has failed due to an unexpected error.");
                            }
                        
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

    }
}
