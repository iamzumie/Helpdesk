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
        #endregion

        #region METHODS
        public ucAddPersoon()
        {
            InitializeComponent();
            Loading = true;
            FillGenderComboBox();
            FillDepartmentComboBox();
            Loading = false;
        }

        private void FillDepartmentComboBox()
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
                cboDepartment.ItemsSource = DT.DefaultView;
                cboDepartment.SelectedValuePath = "DepartmentID";
                cboDepartment.DisplayMemberPath = "Name";
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

        // Vul de combobox van de Gender aan met de keuzes, de binding zit in de XAML om de keuze te laten zien
        private void FillGenderComboBox()
        {
            cboGender.Items.Add("F");
            cboGender.Items.Add("M");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtFirstName.Focus();
        }

        private void ControlsLeegmaken()
        {
            cboDepartment.SelectedItem = -1;
            cboGender.SelectedItem = -1;
            txtFirstName.Text = string.Empty;
            txtMiddleName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtLogin.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtJobTitle.Text = string.Empty;
            txtExtraInfo.Text = string.Empty;
        }
        #endregion

        #region EVENTS
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Random rmd = new Random();
            try
            {
                this.Cursor = Cursors.Wait;
                using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
                {
                    using (SqlCommand CMD = new SqlCommand(Properties.Resources.I_Persoon, CN))
                    {
                        using (SqlDataAdapter DA = new SqlDataAdapter(CMD))
                        {
                            Guid guid = Guid.NewGuid(); // SQL heeft een UNIQUEIDENTIFIER NODIG
                            CMD.CommandType = CommandType.StoredProcedure;
                            CMD.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                            CMD.Parameters.AddWithValue("@MiddleName", txtMiddleName.Text);
                            CMD.Parameters.AddWithValue("@LastName", txtLastName.Text);
                            CMD.Parameters.AddWithValue("@PersonGUID", guid);

                            CMD.Parameters.AddWithValue("@ReturnValue", 0);
                            CMD.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;

                            DT = new DataTable();
                            DA.Fill(DT);

                            if((int)CMD.Parameters["@ReturnValue"].Value == 999)
                            {
                                Loading = true;
                                FillDepartmentComboBox();
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
        #endregion
    }
}
