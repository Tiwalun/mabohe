using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaBoHe
{
    interface ISerialPort
    {
        void Open();
        void Close();

        bool IsOpen { get; }
        string PortName { get; }

        int ReadTimeout { get; set; }
        int WriteTimeout { get; set; }
        void Write(byte[] buffer);
        byte[] ReadBytes(int count);
    }
}
