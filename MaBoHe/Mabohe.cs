using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaBoHe
{
    class Mabohe : INotifyPropertyChanged
    {
        private SerialConn _sc;

        public Mabohe(SerialConn sc)
        {
            _sc = sc;
            _sc.PropertyChanged += handleConnectionChange;
        }

        private void handleConnectionChange(Object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "connectionState")
            {
                PropertyChanged(this, new PropertyChangedEventArgs("isConnected"));

                if (_sc.connectionState == SerialConn.ConnectionState.Connected)
                {
                    updateState();
                }
            }
        }

        private MbState _state;
        public MbState State
        {
            get { return _state; }
        }

        public void togglePower()
        {
            setPower(!State.powerOn);
            updateState();
        }

        public void setPower(bool on)
        {
            Int16 arg;

            if (on)
            {
                arg = 1;
            }
            else
            {
                arg = 0;
            }

            HeaterCommand cmd = HeaterCommand.build(HeaterCommand.commandType.SetPower, arg);
            _sc.sendCommand(cmd);

            System.Threading.Thread.Sleep(1000);
     
            updateState();

            if (State.powerOn != on)
            {
                //setting new power failed
                throw new Commands.CommandException("Failed to set power");
            }
        }

        private MbState getState()
        {
            HeaterCommand cmd = HeaterCommand.build(HeaterCommand.commandType.GetStatus);

            byte[] response = _sc.sendCommand(cmd);

            return MbState.parse(response);
  
        }

        public void updateState()
        {
            _state = getState();
            PropertyChanged(this, new PropertyChangedEventArgs("State"));
        }

        public bool isConnected
        {
            get { return (_sc.connectionState == SerialConn.ConnectionState.Connected); }
        }

        public decimal getSetTemp()
        {
            HeaterCommand cmd = HeaterCommand.build(HeaterCommand.commandType.GetSetTemp);
            byte[] response = _sc.sendCommand(cmd);

            return centiGradeToGrade(ByteToInt(response));
        }

        public void setSetTemp(decimal temp)
        {
            HeaterCommand cmd = HeaterCommand.build(HeaterCommand.commandType.SetTemp, gradeToCentigrade(temp));
            _sc.sendCommand(cmd);
        }

        public decimal getHeatsinkTemp()
        {
            HeaterCommand cmd = HeaterCommand.build(HeaterCommand.commandType.GetTempHeatsink);
            byte[] response = _sc.sendCommand(cmd);

            return centiGradeToGrade(ByteToInt(response));
        }

        public decimal getSensor1Temp()
        {
            HeaterCommand cmd = HeaterCommand.build(HeaterCommand.commandType.GetTempSens1);
            byte[] response = _sc.sendCommand(cmd);

            return centiGradeToGrade(ByteToInt(response));
        }

        public decimal getSensor2Temp()
        {
            HeaterCommand cmd = HeaterCommand.build(HeaterCommand.commandType.GetTempSens2);
            byte[] response = _sc.sendCommand(cmd);

            return centiGradeToGrade(ByteToInt(response));
        }

        private Int16 ByteToInt(byte[] b)
        {
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(b);
            }

            return BitConverter.ToInt16(b, 0);
        }

        private decimal centiGradeToGrade(Int16 g)
        {
            return (Decimal)(g / 10.0);
        }

        private Int16 gradeToCentigrade(decimal temp)
        {
            return (Int16)(temp * 10);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
