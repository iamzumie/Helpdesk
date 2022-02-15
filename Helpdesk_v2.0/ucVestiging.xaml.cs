using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Data;

namespace Helpdesk_v2._0
{
    /// <summary>
    /// Interaction logic for ucVestiging.xaml
    /// </summary>
    public partial class ucVestiging : UserControl
    {
        #region VARIABELEN
        DataTable DT;
        string changed = "Insert";
        Int16 Id = 0;
        bool Loading = true;
        #endregion


        #region METHODS
        public ucVestiging()
        {
            InitializeComponent();
            Loading = true;
            LoadDataGrid();
            Loading = false;
        }

        public class MyDataGrid
        {
            public string BindingId { get; set; }
            public string BindingOmschrijving { get; set; }
            public string BindingGroup { get; set; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtVestiging.Focus();
        }

        // Loading up the DataGrid
        private void LoadDataGrid()
        {
            try
            {
                this.Cursor = Cursors.Wait;
                using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
                {
                    using (SqlCommand CMD = new SqlCommand(Properties.Resources.S_Vestiging, CN))
                    {
                        try
                        {
                            CN.Open();
                            using (SqlDataReader DR = CMD.ExecuteReader(CommandBehavior.CloseConnection))
                            {
                                dgResults.Items.Clear(); // Empties the datagrid first

                                while (DR.Read())
                                {
                                    // Add bindings to the xaml field
                                    dgResults.Items.Add(new MyDataGrid
                                    { 
                                        BindingId = DR["DepartmentID"].ToString(), 
                                        BindingOmschrijving = DR["Name"].ToString(), 
                                        BindingGroup = DR["GroupName"].ToString()
                                    });
                                }
                            }
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

        private void ControlsLeegmaken()
        {
            txtGroup.Text = "";
            txtVestiging.Text = "";
        }
        #endregion


        #region EVENTS
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (changed == "Insert")
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

                else if (changed == "Update")
                {
                    using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
                    {
                        using (SqlCommand CMD = new SqlCommand(Properties.Resources.U_Vestiging, CN))
                        {
                            using (SqlDataAdapter DA = new SqlDataAdapter(CMD))
                            {
                                CMD.CommandType = CommandType.StoredProcedure;
                                CMD.Parameters.AddWithValue("@id", Id);
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
                                    changed = "Insert";
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
                    var data = row.Item as MyDataGrid;

                    txtVestiging.Text = data.BindingOmschrijving;
                    txtGroup.Text = data.BindingGroup;
                    Id = Convert.ToInt16(data.BindingId);
                    changed = "Update";
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
                                CMD.Parameters.AddWithValue("@DepartmentID", Id);
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
                                    changed = "Insert";
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
            if (!Loading)
            {
                DataGridRow DataRow = e.Source as DataGridRow;

                if (DataRow != null)
                {
                    MyDataGrid Data = DataRow.Item as MyDataGrid;
                    Id = Convert.ToInt16(Data.BindingId);
                }
            }
        }
        #endregion
    }
}

