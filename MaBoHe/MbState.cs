using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaBoHe
{
    class MbState
    {
        public bool powerOn { get; set; }
        public bool powerOk { get; set; }
        private bool heatingOn { get; set; }
        private bool fail { get; set; }
        private int errorCode { get; set; }
        public static MbState parse(byte[] response)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            if (response.Length != 1)
                throw new ArgumentOutOfRangeException("Response length is not 1");

            BitArray tmpResponse = new BitArray(response);

            MbState mb = new MbState();

            mb.powerOn = tmpResponse[7];
            mb.powerOk = tmpResponse[6];
            mb.heatingOn = tmpResponse[5];
            mb.fail = tmpResponse[4];
            mb.errorCode = response[0] & 0x0f;

            return mb;

        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (obj is MbState)
            {
                MbState other = (MbState)obj;

                return this.powerOn == other.powerOn &&
                    this.powerOk == other.powerOk &&
                    this.heatingOn == other.heatingOn &&
                    this.fail == other.fail &&
                    this.errorCode == other.errorCode;
            }
            return base.Equals(obj);
        }
    }
}
