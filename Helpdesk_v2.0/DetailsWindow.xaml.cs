using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace Helpdesk_v2._0
{
    /// <summary>
    /// Interaction logic for DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window
    {
        #region VARIABELEN
            DataTable DT;
            bool Loading = true;
        #endregion


        #region METHODS
        public DetailsWindow(int ID)
        {
            InitializeComponent();
            MessageBox.Show(ID.ToString());
            Loading = true;
            FillDepartmentComboBox();
            FillGenderComboBox();
            Loading = false;
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

        // Hier ga ik de ID ophalen die achter mijn combobox zit
        private Int16 GetID ()
        {
            Int16 id = 0;

            using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
            {
                using (SqlCommand CMD = new SqlCommand("select DepartmentID from HumanResources.Department where Name = @Name", CN))
                {
                    CN.Open();
                    CMD.Parameters.Add(new SqlParameter("Name", txtVestiging.Text));
                    using (SqlDataReader DR = CMD.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (DR.Read())
                        {
                            id = (Int16)DR["DepartmentID"];
                        }
                    }
                }
            }
            return id;
        }


        // Hier ga ik de ID ophalen die achter mijn combobox zit
        private string GetPersonID()
        {
            string Name = txtID.Text;
            return Name;
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
                            Int16 DepartmentID = GetID();
                            CMD.CommandType = CommandType.StoredProcedure;
                            CMD.Parameters.AddWithValue("@id", txtID.Text);
                            CMD.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                            CMD.Parameters.AddWithValue("@MiddleName", txtMiddleName.Text);
                            CMD.Parameters.AddWithValue("@LastName", txtLastName.Text);
                            CMD.Parameters.AddWithValue("@LoginID", txtLogin.Text);
                            CMD.Parameters.AddWithValue("@EmailAddress", txtMail.Text);
                            CMD.Parameters.AddWithValue("@JobTitle", txtJobTitle.Text);
                            CMD.Parameters.AddWithValue("@DepartmentID", DepartmentID);
                            CMD.Parameters.AddWithValue("@Gender", cboGender.SelectedItem);
                            CMD.Parameters.AddWithValue("@ReturnValue", 0);
                            CMD.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;

                            DT = new DataTable();
                            DA.Fill(DT);
                        
                            if ((int)CMD.Parameters["@ReturnValue"].Value == 999)
                            {
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
