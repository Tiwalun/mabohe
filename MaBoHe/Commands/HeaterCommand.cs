using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaBoHe
{
    class HeaterCommand
    {
        public enum commandType {
            GetStatus,
            MagicByte,
            GetTempHeatsink,
            GetTempSens1,
            GetTempSens2,
            GetFan,
            GetSetTemp,
            GetVoltage,
            GetCurrent,
            SetPower,
            SetTemp
        }
        private const int _length = 4;
        private const int _offset = 0;
        private int _responseLength;

        private byte[] _param = new byte[] { 0x00, 0x00} ;

        public int length { get { return _length; } }
        public int offset { get { return _offset; } }
        public int responseLength { get { return _responseLength; } }

        private const byte startByte = 0xff;
        private byte cmdByte;

        public byte[] toByte()
        {
            byte[] cmd = new byte[4];
            
            cmd[0] = startByte;
            cmd[1] = cmdByte;
            cmd[2] = _param[0];
            cmd[3] = _param[1];

            return cmd;
        }

        static byte getCommandByte(commandType cmd)
        {
            switch (cmd)
            {
                case commandType.GetStatus:
                    return 0x00;
                case commandType.MagicByte:
                    return 0x01;
                case commandType.GetTempHeatsink:
                    return 0x02;
                case commandType.GetTempSens1:
                    return 0x03;
                case commandType.GetTempSens2:
                    return 0x04;
                case commandType.GetFan:
                    return 0x05;
                case commandType.GetSetTemp:
                    return 0x06;
                case commandType.GetVoltage:
                    return 0x07;
                case commandType.GetCurrent:
                    return 0x08;
                case commandType.SetPower:
                    return 0x20;
                case commandType.SetTemp:
                    return 0x21;
                default:
                    throw new ArgumentOutOfRangeException("cmd", "Unknown command");
            }
        }
        static int getResponseLength(commandType cmd)
        {
            switch (cmd)
            {
                case commandType.GetStatus:
                    return 1;
                case commandType.MagicByte:
                    return 1;
                case commandType.GetTempHeatsink:
                    return 2;
                case commandType.GetTempSens1:
                    return 2;
                case commandType.GetTempSens2:
                    return 2;
                case commandType.GetFan:
                    return 2;
                case commandType.GetSetTemp:
                    return 2;
                case commandType.GetVoltage:
                    return 2;
                case commandType.GetCurrent:
                    return 2;
                case commandType.SetPower:
                    return 0;
                case commandType.SetTemp:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException("cmd");
            }
        }

        public static HeaterCommand build(commandType cmd)
        {
            if (cmd == commandType.SetTemp || cmd == commandType.SetPower) 
            {
                throw new InvalidOperationException();
            } else {
                return new HeaterCommand { cmdByte = getCommandByte(cmd), _responseLength = getResponseLength(cmd)};
	        }
        }
        public static HeaterCommand build(commandType cmd, Int16 param)
        {
            if (cmd == commandType.SetTemp || cmd == commandType.SetPower)
            {
                return new HeaterCommand { cmdByte = getCommandByte(cmd), _responseLength = getResponseLength(cmd), _param = intToByte(param) };
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private static byte[] intToByte(Int16 i)
        {
            byte[] resp = BitConverter.GetBytes(i);

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(resp);
            }

            return resp;
        }
    }
}
