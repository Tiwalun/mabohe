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
            //FakeSerialPortFactory factory = new FakeSerialPortFactory();
            //factory.PortNames.Add("COM1");
            //factory.PortNames.Add("COM2");

            ISerialPortFactory factory = new SerialPortFactory();
            MainWindow mw = new MainWindow();

            MainWindowViewModel vm = new MainWindowViewModel(new SerialConn(new SerialSearcher(factory)), mw);
            
            mw.DataContext = vm;

            mw.Show();
        }
    }
}
