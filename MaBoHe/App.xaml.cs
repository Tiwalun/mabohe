using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MaBoHe
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindowViewModel vm = new MainWindowViewModel();
            MainWindow mw = new MainWindow();
            mw.DataContext = vm;

            mw.Show();
        }
    }
}
