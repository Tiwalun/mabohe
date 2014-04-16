using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace MaBoHe
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private SerialConn sc;
        private Mabohe mbh;
        public static int SYNC_INTERVALL_MS = 1000;
        public static int SAMPLE_POINTS = 200;

        private static string[] _plotProperties = new string[] { "setTemp" };

        private Dictionary<string, OxyPlot.Series.LineSeries> _plotData = new Dictionary<string, LineSeries>();

        private DispatcherTimer _syncTimer = new DispatcherTimer();

        public String connectionState
        {
            get
            {
                switch (sc.connectionState)
                {
                    case SerialConn.ConnectionState.NotConnected:
                        return "Nicht verbunden";
                    case SerialConn.ConnectionState.Connecting:
                        return "Verbinden...";
                    case SerialConn.ConnectionState.Connected:
                        return String.Format("Verbunden ({0})", sc.PortName);
                    case SerialConn.ConnectionState.ConnectionLost:
                        return "Verbindung verloren";
                    default:
                        return "Unbekannter Verbindungsstatus";
                }

            }
        }

        private PlotModel _tempModel;
        public PlotModel tempModel
        {
            get
            {
                return _tempModel;
            }
            set
            {
                if (_tempModel != value)
                {
                    _tempModel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _syncState = "Nicht synchronisiert";
        public String syncState
        {
            get { return _syncState; }
            set
            {
                if (_syncState != value)
                {
                    _syncState = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _connected = false;
        public bool connected
        {
            get
            {
                return _connected;
            }
            set
            {
                if (_connected != value)
                {
                    _connected = value;
                    NotifyPropertyChanged();
                }

            }
        }

        private decimal _setTemp = 20;
        public decimal setTemp
        {
            get
            {
                return _setTemp;
            }
            set
            {
                if (value != _setTemp)
                {
                    _setTemp = value;
                    
                    NotifyPropertyChanged();

                    if (mbh.isConnected)
                    {
                        mbh.setSetTemp(_setTemp);
                    }
                }
            }
        }

        private decimal _tempHeatsinkCentigrade;
        public decimal tempHeatsink
        {
            get
            {
                return _tempHeatsinkCentigrade / 100;
            }
            set
            {
                decimal valCentigrade = value * 100;
                if (valCentigrade != _tempHeatsinkCentigrade)
                {
                    _tempHeatsinkCentigrade = valCentigrade;
                    NotifyPropertyChanged();
                }
            }
        }

        private decimal _tempSensor1;
        public decimal tempSensor1
        {
            get { return _tempSensor1; }
            set
            {
                if (value != _tempSensor1)
                {
                    _tempSensor1 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private decimal _tempSensor2;
        public decimal tempSensor2
        {
            get { return _tempSensor2; }
            set
            {
                if (value != _tempSensor2)
                {
                    _tempSensor2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _fanRPM;
        public int fanRPM
        {
            get
            {
                return _fanRPM;
            }
            set
            {
                if (value != _fanRPM)
                {
                    _fanRPM = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _voltage;
        public int voltage
        {
            get
            {
                return _voltage;
            }
            set
            {
                if (value != _voltage)
                {
                    _voltage = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _powerState = "?";
        public string powerState
        {
            get
            {
                return _powerState;
            }
            set
            {
                if (_powerState != value)
                {
                    _powerState = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public int current { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void SyncAll()
        {
            try
            {
                setTemp = mbh.getSetTemp();
                tempHeatsink = mbh.getHeatsinkTemp();
                tempSensor1 = mbh.getSensor1Temp();
                tempSensor2 = mbh.getSensor2Temp();

                mbh.updateState();

                syncState = "Erfolgreich synchronisiert";
            }
            catch (TimeoutException)
            {
                syncState = "Timeout bei Synchronisation";
            }
        }

        private void handleSyncTick(Object sender, EventArgs e)
        {
            ICommand sc = SyncCommand;

            if (sc.CanExecute(null))
            {
                sc.Execute(null);
            }
        }

        public ICommand ConnectCommand
        {
            get
            {
                return sc.SearchAndConnectCommand;
            }
        }

        public ICommand SyncCommand
        {
            get
            {
                return new Commands.SyncCommand(sc, this);
            }
        }

        public ICommand togglePowerCommand
        {
            get { return new Commands.TogglePowerCommand(mbh); }
        }

        private void updateState(MbState state)
        {
            if (state.powerOn)
            {
                powerState = "On";
            }
            else
            {
                powerState = "Off";
            }
        }

        private void setupPlot()
        {
            tempModel = new PlotModel("Graph Zahl");

            //Axes
            LinearAxis la = new LinearAxis();
            la.Position = AxisPosition.Left;
            la.StringFormat = "#°";

            System.Diagnostics.Debug.WriteLine(la.FormatValue(10));

            tempModel.Axes.Add(la);
            tempModel.Axes.Add(new DateTimeAxis(AxisPosition.Bottom));


            foreach (string prop in _plotProperties)
            {
                //Data Series
                LineSeries ls = new LineSeries(prop);
                _plotData.Add(prop, ls);

                tempModel.Series.Add(ls);
            }

            tempModel.InvalidatePlot(true);



        }

        private void updateGraph(Object sender, PropertyChangedEventArgs e)
        {

            if (_plotData.ContainsKey(e.PropertyName))
            {
                object propVal = this.GetType().GetProperty(e.PropertyName).GetValue(this);

                double val = Convert.ToDouble(propVal);
                _plotData[e.PropertyName].Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, val));

                if (_plotData[e.PropertyName].Points.Count > SAMPLE_POINTS)
                {
                    _plotData[e.PropertyName].Points.RemoveAt(0);
                }
                

            }
        }
        public MainWindowViewModel()
        {
            sc = new SerialConn();
            mbh = new Mabohe(sc);

            setupPlot();

            sc.ConnectionStateChanged += (Object sender, EventArgs e) =>
            {
                    NotifyPropertyChanged("connectionState");

                    if (sc.connectionState == SerialConn.ConnectionState.Connected)
                    {
                        _syncTimer.Start();
                    }
                    else
                    {
                        _syncTimer.Stop();
                    }
            };

            mbh.PropertyChanged += (Object sender, PropertyChangedEventArgs e) =>
                {
                    if (e.PropertyName == "State")
                    {
                        updateState(mbh.State);
                    }
                };

            //setup sync timer
            _syncTimer.Interval = TimeSpan.FromMilliseconds(SYNC_INTERVALL_MS);
            _syncTimer.Tick += handleSyncTick;

            this.PropertyChanged += updateGraph;
        }
    }
}
