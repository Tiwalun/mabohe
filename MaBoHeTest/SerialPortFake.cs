using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaBoHe;

namespace MaBoHeTest
{
    class SerialPortFake : ISerialPort
    {
        bool _open = false;
        byte[] _lastWrite;
        string _name;

        Func<int, byte[]> readBytes;

        public bool IsOpen { get { return _open; } }
        public string PortName { get { return _name; } }
        public void Open()
        {
            _open = true;
        }

        public void Close()
        {
            _open = false;
        }

        public int ReadTimeout { get; set; }
        public int WriteTimeout { get; set; }

        public void Write(byte[] buffer)
        {
            _lastWrite = buffer;
        }


        public byte[] ReadBytes(int count)
        {
            return readBytes.Invoke(count);
        }

        public void setupResponse(Func<int, byte[]> f)
        {
            readBytes = f;
        }
        public SerialPortFake(string name)
        {
            _name = name;
        }
    }
}
