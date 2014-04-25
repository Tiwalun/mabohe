using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaBoHe
{
    class FakeSerialPort : ISerialPort
    {
        private bool _IsOpen;
        public bool IsOpen { get { return _IsOpen; } }

        private string _PortName;
        public string PortName { get { return _PortName; } }

        private byte[] responseBuffer;

        public int ReadTimeout { get; set; }
        public int WriteTimeout { get; set; }

        public void Write(byte[] msg)
        {
            ;
        }
        public byte[] ReadBytes(int count)
        {
            return new byte[count];
        }
        public void Open()
        {
            if (IsOpen)
            {
                throw new InvalidOperationException("Port is already open!");
            }

            _IsOpen = true;
        }

        public void Close()
        {
            _IsOpen = false;
        }

        public FakeSerialPort(string name)
        {
            _PortName = name;
            responseBuffer = new byte[100];
        }
    }
}
