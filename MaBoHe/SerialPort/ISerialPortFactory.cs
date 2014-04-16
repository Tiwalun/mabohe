using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaBoHe
{
    interface ISerialPortFactory
    {
        ISerialPort Create(string name);
        ICollection<string> GetPortNames();
    }
}
