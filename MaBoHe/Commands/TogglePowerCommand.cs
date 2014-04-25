using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MaBoHe.Commands
{
    class TogglePowerCommand : ICommand
    {
        private Mabohe _mb;
        private static Object _sInFlight = new System.Object();

        private static bool _inFlight = false;
        public bool CanExecute(Object parameter)
        {
            return _mb.isConnected && !_inFlight;
        }

        public async void Execute(Object parameter)
        {

            lock(_sInFlight)
            {
                if (_inFlight) throw new InvalidOperationException();

                _inFlight = true;
                CanExecuteChanged(this, null);
            }
            try
            {
                await Task.Run(() => _mb.togglePower());
                _mb.updateState();
            }
            finally
            {
                lock (_sInFlight)
                {
                    _inFlight = false;
                    CanExecuteChanged(this, null);
                }
            }
        }

        public event EventHandler CanExecuteChanged;

        public TogglePowerCommand(Mabohe mb)
        {
            _mb = mb;

            _mb.PropertyChanged += (Object sender, PropertyChangedEventArgs e) =>
            {
                if (CanExecuteChanged != null && e.PropertyName == "isConnected")
                {
                    CanExecuteChanged(this, null);
                }
            };
        }
    }
}
