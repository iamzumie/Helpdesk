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
        #endregion

        public DetailsWindow(object user)
        {
            InitializeComponent();

            DataContext = user;

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection CN = new SqlConnection(Properties.Settings.Default.CN))
            {
                using (SqlCommand CMD = new SqlCommand(Properties.Resources.U_Vestiging, CN))
                {
                    using (SqlDataAdapter DA = new SqlDataAdapter(CMD))
                    {
                        CMD.CommandType = CommandType.StoredProcedure;
                        /*CMD.Parameters.AddWithValue("@id", id);
                        CMD.Parameters.AddWithValue("@Name", txtVestiging.Text);
                        CMD.Parameters.AddWithValue("@GroupName", txtGroup.Text);*/
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
    }
}
