using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MaBoHe.Commands
{
    class ConnectCommand : ICommand
    {
        private SerialConn _conn;

        public bool CanExecute(Object parameter)
        {
            return (_conn.connectionState == SerialConn.ConnectionState.NotConnected);
        }

        public async void Execute(Object parameter)
        {
            await _conn.connectHeaterAsync();
        }

        public event EventHandler CanExecuteChanged;

        public ConnectCommand(SerialConn conn)
        {
            if (conn == null)
            {
                throw new ArgumentNullException("conn");
            }

            _conn = conn;

            _conn.PropertyChanged += (Object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == "connectionState" && CanExecuteChanged != null)
                {
                    CanExecuteChanged(this, null);
                }
            };
        }


    }
}
