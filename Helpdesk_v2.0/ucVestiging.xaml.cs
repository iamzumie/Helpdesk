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
using System.Data.SqlClient;
using System.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Hangfire.Annotations;

namespace Helpdesk_v2._0
{
    /// <summary>
    /// Interaction logic for ucVestiging.xaml
    /// </summary>
    public partial class ucVestiging : UserControl
    {
        #region VARIABELEN
            DataTable DT;
            string Changed = "Insert";
            Int16 id = 0;
            bool Loading = true;
        #endregion

        #region METHODS
        public ucVestiging()
        {
            InitializeComponent();
            Loading = true;
            LoadDataGrid(); // Moet dit hier of beter bij de UserControl_Loaded?
            Loading = false;
        }

        public class MyItem
        {
            public string id { get; set; }
            public string omschrijving { get; set; }
            public string group { get; set; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtVestiging.Focus();
        }

        // Loading up the DataGrid
        private void LoadDataGrid()
        {
            // Enkel Select statement = beter om dan ExecuteReader te gebruiken?
            string Q = "select * from HumanResources.Department";

            try
            {
                this.Cursor = Cursors.Wait;
                using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
                {
                    using (SqlCommand CMD = new SqlCommand(Q, CN))
                    {
                        try
                        {
                            CN.Open();
                            using (SqlDataReader DR = CMD.ExecuteReader())
                            {
                                FillDataGrid(DR);
                            }
                            CN.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There has been a mistake loading the list" + ex.ToString(), "Error", MessageBoxButton.OK);
            }
            finally
            {
                this.Cursor = null;
            }
        }

        private void FillDataGrid(SqlDataReader Reader)
        {
            // Maakt de datagrid eerst leeg
            dgResults.Items.Clear();

            while (Reader.Read())
            {
                string DepartmentID, Name, GroupName = string.Empty;

                DepartmentID = Reader["DepartmentID"].ToString();
                Name = Reader["Name"].ToString();
                GroupName = Reader["GroupName"].ToString();

                dgResults.Items.Add(new MyItem { id = DepartmentID, omschrijving = Name, group = GroupName });
            }
        }
        private void ControlsLeegmaken()
        {
            txtGroup.Text = string.Empty;
            txtVestiging.Text = string.Empty;
        }
        #endregion

        #region EVENTS
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Changed == "Insert")
                {
                    using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
                    {
                        using (SqlCommand CMD = new SqlCommand(Properties.Resources.I_Vestiging, CN))
                        {
                            using (SqlDataAdapter DA = new SqlDataAdapter(CMD))
                            {
                                CMD.CommandType = CommandType.StoredProcedure;
                                CMD.Parameters.AddWithValue("@Name", txtVestiging.Text);
                                CMD.Parameters.AddWithValue("@GroupName", txtGroup.Text);
                                CMD.Parameters.AddWithValue("@ReturnValue", 0);
                                CMD.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;

                                DT = new DataTable();
                                DA.Fill(DT);

                                if ((int)CMD.Parameters["@ReturnValue"].Value == 999)
                                {
                                    Loading = true;
                                    LoadDataGrid();
                                    Loading = false;
                                    ControlsLeegmaken();
                                }
                                else if ((int)CMD.Parameters["@ReturnValue"].Value == 998)
                                {
                                    MessageBox.Show("The insert has failed due to concurrency issues.");
                                }
                                else if ((int)CMD.Parameters["@ReturnValue"].Value == 997)
                                {
                                    MessageBox.Show("The insert has failed due to an unexpected error.");
                                }
                            }
                        }
                    }
                }

                else if (Changed == "Update")
                {
                    using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
                    {
                        using (SqlCommand CMD = new SqlCommand(Properties.Resources.U_Vestiging, CN))
                        {
                            using (SqlDataAdapter DA = new SqlDataAdapter(CMD))
                            {
                                CMD.CommandType = CommandType.StoredProcedure;
                                CMD.Parameters.AddWithValue("@id", id);
                                CMD.Parameters.AddWithValue("@Name", txtVestiging.Text);
                                CMD.Parameters.AddWithValue("@GroupName", txtGroup.Text);
                                CMD.Parameters.AddWithValue("@ReturnValue", 0);
                                CMD.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;

                                DT = new DataTable();
                                DA.Fill(DT);

                                if ((int)CMD.Parameters["@ReturnValue"].Value == 999)
                                {
                                    Loading = true;
                                    LoadDataGrid();
                                    Loading = false;
                                    ControlsLeegmaken();
                                    Changed = "Insert";
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var row = e.Source as DataGridRow;
                if (row != null)
                {
                    var data = row.Item as MyItem;

                    txtVestiging.Text = data.omschrijving;
                    txtGroup.Text = data.group;
                    id = Convert.ToInt16(data.id);
                    Changed = "Update";
                }
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.Close();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Ben je zeker dat je deze vestiging wilt verwijderen?","Verwijderen", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (dgResults.SelectedItems.Count > 0)
                {
                    using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
                    {
                        using (SqlCommand CMD = new SqlCommand(Properties.Resources.D_Vestiging, CN))
                        {
                            using (SqlDataAdapter DA = new SqlDataAdapter(CMD))
                            {
                                CMD.CommandType = CommandType.StoredProcedure;
                                CMD.Parameters.AddWithValue("@DepartmentID", id);
                                CMD.Parameters.AddWithValue("@ReturnValue", 0);
                                CMD.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;

                                DT = new DataTable();
                                DA.Fill(DT);

                                if ((int)CMD.Parameters["@ReturnValue"].Value == 999)
                                {
                                    Loading = true;
                                    LoadDataGrid();
                                    Loading = false;
                                    ControlsLeegmaken();
                                    Changed = "Insert";
                                    txtVestiging.Focus();
                                }
                                else if ((int)CMD.Parameters["@ReturnValue"].Value == 998)
                                {
                                    MessageBox.Show("The delete has failed due to concurrency issues.");
                                }
                                else if ((int)CMD.Parameters["@ReturnValue"].Value == 997)
                                {
                                    MessageBox.Show("The delete has failed due to an unexpected error.");
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DataGridRow_Selected(object sender, RoutedEventArgs e)
        {
            if (Loading == false)
            {
                var row = e.Source as DataGridRow;
                if (row != null)
                {
                    var data = row.Item as MyItem;
                    id = Convert.ToInt16(data.id);
                }
            }
        }
        #endregion
    }
}

