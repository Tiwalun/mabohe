using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MaBoHe
{
    class SerialConn : INotifyPropertyChanged
    {
        public enum ConnectionState
        {
            NotConnected,
            Connecting,
            Connected,
            ConnectionLost
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private int _timeoutCount = 0;
        private int _timeoutThreshold = 10;
        private void NotifyPropertChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private SerialPort _sp;
        private Object _spAccess = new System.Object();

        private ConnectionState _connectionState = ConnectionState.NotConnected;
        public ConnectionState connectionState
        {
            get
            {
                return _connectionState;
            }
            internal set
            {
                if (value != _connectionState)
                {
                    _connectionState = value;
                    NotifyPropertChanged();
                }
            }
        }

        public string PortName
        {
            get
            {
                if (connectionState == ConnectionState.Connected)
                {
                    return _sp.PortName;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public void connect(SerialPort sp)
        {
            if (sp == null)
            {
                throw new ArgumentNullException("sp");
            }

            if (!sp.IsOpen)
            {
                sp.Open();
            }

            _sp = sp;

            connectionState = ConnectionState.Connected;
        }

        private void logErrors(SerialError e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("SerialPort Error: {0}", e));
        }

        public byte[] sendCommand(HeaterCommand cmd)
        {
            byte[] result;

            try
            {
                lock (_spAccess)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Bytes in in buffer before write: {0}", _sp.BytesToRead));

                    _sp.Write(cmd.toByte(), cmd.offset, cmd.length);

                    result = new byte[cmd.responseLength];

                    System.Diagnostics.Debug.WriteLine(String.Format("Bytes in in buffer after before read: {0}", _sp.BytesToRead));
                    
                    for (int i = 0; i < cmd.responseLength; i++)
                    {
                        result[i] = (byte)_sp.ReadByte();
                    }

                    System.Diagnostics.Debug.WriteLine(String.Format("Bytes in in buffer after read: {0}", _sp.BytesToRead));
                }
            } 
            catch (TimeoutException)
            {
                registerTimeout();
                throw;
            }

            _timeoutCount = 0;
            return result;
        }

        private void registerTimeout()
        {
            _timeoutCount += 1;

            if (_timeoutCount > _timeoutThreshold)
            {
                _connectionState = ConnectionState.ConnectionLost;
                System.Diagnostics.Debug.WriteLine("Too many timeouts, connection closed");
            }
        }

        public async Task<bool> connectHeaterAsync()
        {
            connectionState = ConnectionState.Connecting;

            SerialPort sp = await SerialSearcher.searchHeaterAsync();

            if (sp != null)
            {
                connect(sp);
                return true;
            }
            else
            {
                connectionState = ConnectionState.NotConnected;
                return false;
            }
        }

        public ICommand connectCommand
        {
            get { return new Commands.ConnectCommand(this);  }
        }
    }
}
