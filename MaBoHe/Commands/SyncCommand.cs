using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MaBoHe.Commands
{
    class SyncCommand : ICommand
    {

        private SerialConn _sc;
        private MainWindowViewModel _vm;
 

        public bool CanExecute(Object parameter)
        {
            return _sc.connectionState == SerialConn.ConnectionState.Connected;
        }

        public async void Execute(Object parameter)
        {
            await Task.Run(() =>_vm.SyncAll());
        }

        public event EventHandler CanExecuteChanged;

        public SyncCommand(SerialConn sc, MainWindowViewModel vm)
        {
            _sc = sc;
            _vm = vm;

            _sc.PropertyChanged += (Object sender, PropertyChangedEventArgs e) =>
                {
                    if (e.PropertyName == "connectionState" && CanExecuteChanged != null)
                    {
                        CanExecuteChanged(this, null);
                    }
                };
        }
    }
}
