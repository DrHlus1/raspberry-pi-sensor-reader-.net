using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace meteostation
{
    [Serializable]
    public class OneWireModuleNotLoadedException : System.Exception
    {
        public OneWireModuleNotLoadedException() : base() { }
        public OneWireModuleNotLoadedException(string message) : base(message) { }
        public OneWireModuleNotLoadedException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected OneWireModuleNotLoadedException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}
