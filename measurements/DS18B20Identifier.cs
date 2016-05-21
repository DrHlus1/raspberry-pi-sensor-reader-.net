using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace meteostation
{
    [Serializable]
    public struct DS18B20Identifier
    {
        public DS18B20Identifier(Int64 fullIdentifier)
        {
            //Zeroing all unneeded parts
            serialCode_ = fullIdentifier & 0xFFFFFFFFFFFF;
            Int64 family = fullIdentifier >> 48;
            family_ = (Byte)(family & 0xFF);
        }

        public DS18B20Identifier(Byte family, Int64 serialCode)
        {
            family_ = family;
            //TODO serialCode should have at most 48 meaningful bits. Throw exception if there's something there
            serialCode_ = serialCode;
        }

        public Byte Family
        {
            get
            {
                return family_;
            }
            private set
            {
                family_ = value;
            }
        }

        public Int64 SerialCode
        {
            get
            {
                return serialCode_;
            }
            private set
            {
                serialCode_ = value;
            }
        }

        public Int64 ToInt64()
        {
            UInt64 family = Family;
            UInt64 serial = (UInt64)SerialCode;
            return (Int64)((family << 48) | serial);
        }

        private Byte family_;
        private Int64 serialCode_;
    }
}
