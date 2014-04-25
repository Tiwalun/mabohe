using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaBoHe.HeaterCommands;

namespace MaBoHe
{
    class SerialSearcher
    {
        public const byte magicResponse = 0x23;

        private readonly ISerialPortFactory serialPortFactory;

        private ISerialPort createPort(string name)
        {
            return serialPortFactory.Create(name);
        }

        private bool CheckIfHeater(ISerialPort port)
        {
            if (!port.IsOpen)
            {
                throw new ArgumentException("The serial port has to be open to check if the heater is connected to it.", "port");
            }

            HeaterCommand cmd = HeaterCommand.build(HeaterCommand.commandType.MagicByte);
            byte[] buff;

            try
            {
                port.Write(cmd.toByte());

                buff = port.ReadBytes(1);
            }
            catch (TimeoutException)
            {
                return false;
            }

            return buff[0] == magicResponse;
        }

        private ISerialPort OpenIfHeater(string port)
        {
            ISerialPort sp = createPort(port);

            bool IsHeater = false;

            try
            {
                sp.Open();

                if (!sp.IsOpen)
                {
                    return null;
                }

                IsHeater = CheckIfHeater(sp);

            }

            catch (UnauthorizedAccessException)
            {
                return null;
            }
            finally
            {
                if (!IsHeater)
                {
                    sp.Close();
                }
            }

            if (IsHeater) { return sp; }
            else { return null; }
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

        public SerialSearcher(ISerialPortFactory factory)
        {
            serialPortFactory = factory;
        }
    }
}
