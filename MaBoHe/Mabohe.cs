using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaBoHe.HeaterCommands;

namespace MaBoHe
{
    class Mabohe : INotifyPropertyChanged
    {
        private readonly SerialConn _sc;
        private MbState _state;

        private bool _requestedPowerOn;

        public PowerState PowerState
        {
            get
            {
                if (_state == null) { return MaBoHe.PowerState.Unknown;}

                if (_state.powerOn && !_state.powerOk)
                {
                    return MaBoHe.PowerState.PowerFailure;
                }

                if (_state.powerOn != _requestedPowerOn)
                {
                    return MaBoHe.PowerState.PowerChangin;
                }

                if (_state.powerOn)
                {
                    return MaBoHe.PowerState.PowerOn;
                }
                else
                {
                    return MaBoHe.PowerState.PowerOff;
                }
            }
        }

        public Mabohe(SerialConn sc)
        {
            _sc = sc;
            _sc.ConnectionStateChanged += handleConnectionChange;
        }

        private MbState getState()
        {
            HeaterCommand cmd = HeaterCommand.build(HeaterCommand.commandType.GetStatus);

            byte[] response = _sc.sendCommand(cmd);

            return MbState.parse(response);

        }

        private void handleConnectionChange(Object sender, EventArgs e)
        {

            PropertyChanged(this, new PropertyChangedEventArgs("isConnected"));

            if (_sc.connectionState == SerialConn.ConnectionState.Connected)
            {
                updateState();
                _requestedPowerOn = _state.powerOn;
            }

        }

        public MbState State
        {
            get { return _state; }
            private set
            {
                if (State == null || !State.Equals(value))
                {
                    _state = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("State"));
                    PropertyChanged(this, new PropertyChangedEventArgs("PowerState"));
                }
            }
        }

        public void togglePower()
        {
            setPower(!State.powerOn);
        }

        public void setPower(bool on)
        {
            //If the power is already on, don't bother changing it.
            if (_state.powerOn == on)
            {
                return;
            }

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
        }

        public void updateState()
        {
            State = getState();
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

    enum PowerState
    {
        PowerOff,
        PowerOn,
        PowerChangin,
        PowerFailure,
        Unknown
    }
}
