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
    class SerialConn
    {
        public enum ConnectionState
        {
            NotConnected,
            Connecting,
            Connected,
            ConnectionLost
        }

        public event EventHandler ConnectionStateChanged;

        public SerialSearcher searcher = new SerialSearcher();

        private int _timeoutCount = 0;
        private int _timeoutThreshold = 10;

        private ISerialPort _sp;
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
                    ConnectionStateChanged(this, null);
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

        public void connect(ISerialPort sp)
        {
            if (connectionState == ConnectionState.Connected)
            {
                throw new InvalidOperationException();
            }

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

        public void disconnect()
        {
            connectionState = ConnectionState.NotConnected;
            _sp = null;
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

                    _sp.Write(cmd.toByte());
                    result =_sp.ReadBytes(cmd.responseLength);

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
            if (connectionState == ConnectionState.Connected)
            {
                throw new InvalidOperationException("Already connected!");
            }

            connectionState = ConnectionState.Connecting;

            ISerialPort sp = await searcher.searchHeaterAsync();

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

        public ICommand SearchAndConnectCommand
        {
            get { return new Commands.SearchHeaterAndConnectCommand(this);  }
        }
    }
}
