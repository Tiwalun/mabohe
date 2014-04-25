using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaBoHe
{
    class FakeSerialPortFactory : ISerialPortFactory
    {

        List<string> _portNames = new List<string>();

        public ICollection<string> PortNames
        {
            get { return _portNames; }
        }

        public ICollection<string> GetPortNames()
        {
            return _portNames;
        }

        public ISerialPort Create(string name)
        {

            return new FakeSerialPort(name);
        }
    }
}
