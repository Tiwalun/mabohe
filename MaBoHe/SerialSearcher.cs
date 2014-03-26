using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace MaBoHe
{
    class SerialSearcher
    {
        private const byte magicResponse = 0x23;

        private static SerialPort createPort(string name)
        {
            return new SerialPort(name, 9600, Parity.None, 8, StopBits.One) { ReadTimeout = 4000, WriteTimeout = 500 };
        }

        private static SerialPort OpenIfHeater(string port)
        {
            using (SerialPort sp = createPort(port))
            {
                try
                {
                    sp.Open();

                    if (!sp.IsOpen)
                    {
                        return null;
                    }

                    HeaterCommand cmd = HeaterCommand.build(HeaterCommand.commandType.MagicByte);
                    byte[] buff = new byte[1];

                    try
                    {
                        sp.Write(cmd.toByte(), cmd.offset, cmd.length);

                        sp.Read(buff, 0, 1);
                    }
                    catch (TimeoutException)
                    {
                        sp.Close();
                        return null;
                    }

                    if (buff[0] == magicResponse)
                    {
                        return sp;
                    }
                    else
                    {
                        return null;
                    }
                }

                catch (UnauthorizedAccessException)
                {
                    return null;
                }

            }
        }

        private static async Task<SerialPort> OpenIfHeaterAsync(string port)
        {
            return await Task.Run(() => OpenIfHeater(port));
        }

        public static SerialPort searchHeater()
        {

            string[] ports = SerialPort.GetPortNames();

            if (ports.Length == 0)
            {
                return null;
            }


            IList<Task<SerialPort>> openQuery = (from port in ports select OpenIfHeaterAsync(port)).ToList();

            Task<SerialPort> foundHeater;

            while (openQuery.Count > 0)
            {
                foundHeater = Task.WhenAny(openQuery).Result;

                if (foundHeater.Status == TaskStatus.RanToCompletion && foundHeater.Result != null)
                {
                    //TODO: Cancel other tasks?
                    return foundHeater.Result;
                }
                else
                {
                    openQuery.Remove(foundHeater);
                }
            }

            return null;
        }

        public static async Task<SerialPort> searchHeaterAsync()
        {
            return await Task.Run(() => searchHeater());
        }
    }
}
