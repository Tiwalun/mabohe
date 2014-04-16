using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaBoHe
{
    class SerialSearcher
    {
        public const byte magicResponse = 0x23;

        public ISerialPortFactory serialPortFactory = new SerialPortFactory();

        private ISerialPort createPort(string name)
        {
            return serialPortFactory.Create(name);
        }

        private ISerialPort OpenIfHeater(string port)
        {
            ISerialPort sp = createPort(port);

                try
                {
                    sp.Open();

                    if (!sp.IsOpen)
                    {
                        return null;
                    }

                    HeaterCommand cmd = HeaterCommand.build(HeaterCommand.commandType.MagicByte);
                    byte[] buff;

                    try
                    {
                        sp.Write(cmd.toByte());

                        buff = sp.ReadBytes(1);
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

        private async Task<ISerialPort> OpenIfHeaterAsync(string port)
        {
            return await Task.Run(() => OpenIfHeater(port));
        }

        public ISerialPort searchHeater()
        {

            ICollection<string> ports = serialPortFactory.GetPortNames();

            if (ports.Count == 0)
            {
                return null;
            }


            IList<Task<ISerialPort>> openQuery = (from port in ports select OpenIfHeaterAsync(port)).ToList();

            Task<ISerialPort> foundHeater;

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

        public async Task<ISerialPort> searchHeaterAsync()
        {
            return await Task.Run(() => searchHeater());
        }
    }
}
