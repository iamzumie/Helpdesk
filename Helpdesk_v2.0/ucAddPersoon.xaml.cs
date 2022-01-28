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
    /// Interaction logic for ucAddPersoon.xaml
    /// </summary>
    public partial class ucAddPersoon : UserControl
    {
        #region VARIABELEN
        DataTable DT;
        bool Loading = true;
        DataRow RW;
        #endregion

        public ucAddPersoon()
        {
            InitializeComponent();
            Loading = true;
            BindComboBox();
            Loading = false;

        }


        private void BindComboBox()
        {
            // String
            string Q = "SELECT * FROM HumanResources.Department Order by Name asc";

            try
            {
                this.Cursor = Cursors.Wait;
                using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
                {
                    using(SqlCommand CMD = new SqlCommand(Q, CN))
                    {
                        using (SqlDataAdapter DA = new SqlDataAdapter(CMD))
                        {
                            try
                            {
                                DT = new DataTable();
                                DA.Fill(DT);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }

                        }
                    }
                }
                cboDepartement.ItemsSource = DT.DefaultView;
                cboDepartement.SelectedValuePath = "DepartmentID";
                cboDepartement.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er heeft zich een fout voorgedaan bij het opvullen van mijn lijst." + ex.ToString(), "Opvul fout.", MessageBoxButton.OK);
            }
            finally
            {
                this.Cursor = null;
            }
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtNaam.Focus();
        }

        private void ControlsLeegmaken()
        {
            cboDepartement.SelectedItem = -1;
            cboGender.SelectedItem = -1;
            txtLogin.Text = string.Empty;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            string Q = @"INSERT INTO HumanResources.Employee(BusinessEntityID, NationalIDNumber, BirthDate, MaritalStatus, LoginID, Gender, HireDate, JobTitle) VALUES(@BusinessEntityID, @NationalIDNumber, @BirthDate, @MaritalStatus, @LoginID, @Gender, @HireDate, @Departement);";
            Random rmd = new Random();

            try
            {
                this.Cursor = Cursors.Wait;
                using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
                {
                    using (SqlCommand CMD = new SqlCommand(Q, CN))
                    {
                        using (SqlDataAdapter DA = new SqlDataAdapter(CMD))
                        {

                            CMD.Parameters.AddWithValue("@BusinessEntityID", 1707);
                            CMD.Parameters.AddWithValue("@NationalIDNumber", rmd.Next());
                            CMD.Parameters.AddWithValue("@BirthDate", "1977-06-06");
                            CMD.Parameters.AddWithValue("@MaritalStatus", "S");
                            CMD.Parameters.AddWithValue("@LoginID", txtLogin.Text);
                            CMD.Parameters.AddWithValue("@Gender", 'F');
                            CMD.Parameters.AddWithValue("@Departement", (Int16)cboDepartement.SelectedValue);
                            CMD.Parameters.AddWithValue("@HireDate", "2011-02-15");
                            CMD.Parameters.AddWithValue("@ReturnValue", 0);
                            CMD.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;

                            DT = new DataTable();
                            DA.Fill(DT);

                            if((int)CMD.Parameters["@ReturnValue"].Value == 999)
                            {
                                Loading = true;
                                BindComboBox();
                                Loading = false;
                                ControlsLeegmaken();
                            }
                            else if((int)CMD.Parameters["@ReturnValue"].Value == 998)
                            {
                                MessageBox.Show("De insert is niet gelukt, door een concurrency probleem.");
                            }
                            else if ((int)CMD.Parameters["@ReturnValue"].Value == 997)
                            {
                                MessageBox.Show("De insert is niet gelukt, door een onvoorziene fout.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
