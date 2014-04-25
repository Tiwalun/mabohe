using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MaBoHe;

namespace MaBoHeTest
{
    [TestClass]
    public class SerialSearcherTest
    {
        [TestMethod]
        public void TestSearchWithNoHeater()
        {
            ISerialPort spNoHeater = new MaBoHe.Fakes.StubISerialPort()
            {
                PortNameGet = () => "Port 1",
                ReadBytesInt32 = (count) => { return new byte[] { 5, 6 }; }
            };

            ISerialPortFactory spf = new MaBoHe.Fakes.StubISerialPortFactory()
            {
                GetPortNames = () => { return new string[] { "Port 1" }; },
                CreateString = (name) => { if (name == "Port 1") return spNoHeater; else return null; }
            };


            SerialSearcher search = new SerialSearcher(spf);

            Assert.IsNull(search.searchHeater());

        }

        [TestMethod]
        public void TestSearchWithHeater()
        {
            

            SerialPortFake spHeater = new SerialPortFake("Port 1");
            spHeater.setupResponse((count) => { return new byte[] { SerialSearcher.magicResponse }; });

            ISerialPortFactory spf = new MaBoHe.Fakes.StubISerialPortFactory()
            {
                GetPortNames = () => { return new string[] { "Port 1" }; },
                CreateString = (name) => { if (name == "Port 1") return spHeater; else return null; }
            };

            SerialSearcher search = new SerialSearcher(spf);

            Assert.AreEqual(spHeater, search.searchHeater());

        }
    }
}
