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

        private bool _inFlight = false;
        private Object _sInFlight = new System.Object();
 

        public bool CanExecute(Object parameter)
        {
            lock (_sInFlight)
            {
                return _sc.connectionState == SerialConn.ConnectionState.Connected && !_inFlight;
            }
        }

        public async void Execute(Object parameter)
        {
            lock (_sInFlight)
            {
                _inFlight = true;
            }
            try
            {
                await Task.Run(() => _vm.SyncAll());
            }
            finally
            {
                _inFlight = false;
            }
        }

        public event EventHandler CanExecuteChanged;

        public SyncCommand(SerialConn sc, MainWindowViewModel vm)
        {
            _sc = sc;
            _vm = vm;

            _sc.ConnectionStateChanged += (Object sender, EventArgs e) =>
                {
                    if (CanExecuteChanged != null)
                    {
                        CanExecuteChanged(this, null);
                    }
                };
        }
    }
}
