using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MaBoHe.Commands
{
    class SearchHeaterAndConnectCommand : ICommand
    {
        private readonly SerialConn _conn;

        public bool CanExecute(Object parameter)
        {
            return (_conn.connectionState == SerialConn.ConnectionState.NotConnected);
        }

        public async void Execute(Object parameter)
        {
            await _conn.connectHeaterAsync();
        }

        public event EventHandler CanExecuteChanged;

        public SearchHeaterAndConnectCommand(SerialConn conn)
        {
            if (conn == null)
            {
                throw new ArgumentNullException("conn");
            }

            _conn = conn;

            _conn.ConnectionStateChanged += (Object sender, EventArgs e) =>
            {
                if (CanExecuteChanged != null)
                {
                    CanExecuteChanged(this, null);
                }
            };
        }


    }
}
