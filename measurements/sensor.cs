using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace meteostation
{
    [Serializable]
    public abstract class Sensor
    {
        public Sensor()
        {
        }

        public Sensor(Int64 id, String shortDescription)
        {
            ID = id;
            ShortDescription = shortDescription;
        }

        public Int64 ID
        {
            get
            {
                return id_;
            }
            protected set
            {
                ValidateID(value);
                id_ = value;
            }
        }

        public String ShortDescription
        {
            get
            {
                return shortDescription_;
            }
            protected set
            {
                shortDescription_ = value;
            }
        }

        public abstract List<Measurement> ReadMeasurements();

        virtual protected void ValidateID(Int64 id)
        {
        }

        private String shortDescription_;
        private Int64 id_;
    }
}
