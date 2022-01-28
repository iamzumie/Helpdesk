using Helpdesk_v2._0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Helpdesk_v3._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // This makes the menu item go to the right hand instead of left hand
            var menuDropAlignmentField = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
            Action setAlignmentValue = () => {
                if (SystemParameters.MenuDropAlignment && menuDropAlignmentField != null) menuDropAlignmentField.SetValue(null, false);
            };
            setAlignmentValue();
            SystemParameters.StaticPropertyChanged += (sender, e) => { setAlignmentValue(); };
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void OpenUsercontrol(UserControl uc)
        {
            if (grdMain.Children.Count > 1)
            {
                grdMain.Children.RemoveAt(1);
            }
            Grid.SetColumn(uc,0);
            Grid.SetRow(uc,1);
            grdMain.Children.Add(uc);
        }

        private void mnuEmployeeLijst_Click(object sender, RoutedEventArgs e)
        {
            ucEmployees _ucEmployees = new ucEmployees();
            OpenUsercontrol(_ucEmployees);
        }

        private void mnuEployeeAdd_Click(object sender, RoutedEventArgs e)
        {
            ucAddPersoon _ucAddPersoon = new ucAddPersoon();
            OpenUsercontrol(_ucAddPersoon);
        }

        private void mnuVestigingLijst_Click(object sender, RoutedEventArgs e)
        {
            ucVestiging _ucVestiging = new ucVestiging();
            OpenUsercontrol(_ucVestiging);
        }
    }
}
