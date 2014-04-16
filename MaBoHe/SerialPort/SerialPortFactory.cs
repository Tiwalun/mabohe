using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace MaBoHe
{
    class SerialPortFactory : ISerialPortFactory
    {
        public int baudRate { get; set; }

        public Parity parity { get; set; }

        public int dataBits { get; set; }

        public StopBits stopBits { get; set; }

        public SerialPortFactory()
        {
            baudRate = 9600;
            parity = Parity.None;
            dataBits = 8;
            stopBits = StopBits.One;
        }

        public ISerialPort Create(string name)
        {
            SerialPort sp = new SerialPort(name, baudRate, parity, dataBits, stopBits) { ReadTimeout = 4000, WriteTimeout = 500 };
            return new SerialPortWrapper(sp);
        }

        public ICollection<string> GetPortNames()
        {
            return SerialPort.GetPortNames();
        }
    }
}
