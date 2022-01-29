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
        #endregion

        public ucVestiging()
        {
            InitializeComponent();
            LoadDataGrid();
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
            // Laad vestigingen, use of SqlDataReader because of select statement
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
                                FillDataGrid(DR); // Fills the Datagrid with the data from the DataReader
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
                                    LoadDataGrid();
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
                                CMD.Parameters.AddWithValue("@Name", txtVestiging.Text);
                                CMD.Parameters.AddWithValue("@GroupName", txtGroup.Text);
                                CMD.Parameters.AddWithValue("@id", id);
                                CMD.Parameters.AddWithValue("@ReturnValue", 0);
                                CMD.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;

                                DT = new DataTable();
                                DA.Fill(DT);

                                if ((int)CMD.Parameters["@ReturnValue"].Value == 999)
                                {
                                    LoadDataGrid();
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

        private void ControlsLeegmaken()
        {
            txtGroup.Text = string.Empty;
            txtVestiging.Text = string.Empty;
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
            if (dgResults.SelectedItems.Count > 0)
            {
                var row = dgResults.SelectedItem as MyItem;

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
                                LoadDataGrid();
                                ControlsLeegmaken();
                                Changed = "Insert";
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

        private void DataGridRow_Selected(object sender, RoutedEventArgs e)
        {
                var row = e.Source as DataGridRow;
                if (row != null)
                {
                    var data = row.Item as MyItem;
                    id = Convert.ToInt16(data.id);
                }
            }
    }
}

