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
        DataRow DR;
        #endregion

        public ucVestiging()
        {
            InitializeComponent();
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

            // Laad vestigingen
            string Q = "select * from HumanResources.Department";

            try
            {
                using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
                {
                    using (SqlCommand CMD = new SqlCommand(Q, CN))
                    {
                        CN.Open();
                        using (SqlDataReader DR = CMD.ExecuteReader())
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
            string Q = "Insert into HumanResources.Department (Name, GroupName) Values (@Name, @GroupName)";
            try
            {
                using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
                {
                    using (SqlCommand CMD = new SqlCommand(Q, CN))
                    {
                        CN.Open();
                        using (SqlDataAdapter DA = new SqlDataAdapter(CMD))
                        {
                            CMD.Parameters.AddWithValue("@Name", txtVestiging.Text);
                            CMD.Parameters.AddWithValue("@GroupName", txtGroup.Text);

                            DT = new DataTable();
                            DA.Fill(DT);

                            ControlsLeegmaken();
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

        private void ControlsLeegmaken()
        {
            txtGroup.Text = string.Empty;
            txtVestiging.Text = string.Empty;
        }

    }
}

