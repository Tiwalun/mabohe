using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace MaBoHe
{
    class SerialPortWrapper : ISerialPort
    {
        SerialPort _sp;
        public SerialPortWrapper(SerialPort sp)
        {
            _sp = sp;
        }

        public bool IsOpen { get { return _sp.IsOpen; } }
        public string PortName { get { return _sp.PortName; } }

        public void Open()
        {
            _sp.Open();
        }

        public void Close()
        {
            _sp.Close();
        }

        public int WriteTimeout
        {
            get { return _sp.WriteTimeout; }
            set { _sp.WriteTimeout = value; }
        }

        public int ReadTimeout
        {
            get { return _sp.ReadTimeout; }
            set { _sp.ReadTimeout = value; }
        }

        public void Write(byte[] buffer)
        {
            _sp.Write(buffer, 0, buffer.Length);
        }

        public byte[] ReadBytes(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "Count has to be greater or equal to zero");
            }

            byte[] buffer = new byte[count];

            for (int i = 0; i < count; i++)
            {
                buffer[i] = (byte) _sp.ReadByte();
            }

            return buffer;
        }
    }
}
