using Image_Backuper_GUI.ViewModel;
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
using System.Windows.Shapes;

namespace Image_Backuper_GUI.View
{
    /// <summary>
    /// Interaction logic for SettingsTab.xaml
    /// </summary>
    public partial class SettingsTab : TabItem
    {
        public SettingsTab()
        {
            InitializeComponent();
            viewModel = new SettingsViewModel();
            DataContext = viewModel;
        }

        private SettingsViewModel viewModel { get; set; }

        private void ListGotFocus(object sender, RoutedEventArgs e)
        {
            remove.IsEnabled = true;
        }

        private void removeClick(object sender, RoutedEventArgs e)
        {
            viewModel.DeleteHandler((string)dirList.SelectedValue);
            remove.IsEnabled = false;
            dirList.SelectedIndex = -1;
        }
    }
}
