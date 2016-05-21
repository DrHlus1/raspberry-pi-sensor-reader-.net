using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace meteostation
{
    [Serializable]
    public abstract class Measurement
    {
        public Measurement()
        {
        }

        //TODO use abstract Sensor class in this constructor
        public Measurement(SensorDS18B20 sensor, Double value, DateTime timestamp)
        {
            Sensor = sensor;
            Value = value;
            Timestamp = timestamp;
        }

        public Sensor Sensor
        {
            get { return sensor_; }
            protected set { sensor_ = value; }
        }

        public Double Value
        {
            get { return value_; }
            protected set 
            {
                ValidateValue(value);
                value_ = value; 
            }
        }

        public DateTime Timestamp
        {
            get { return timestamp_; }
            set
            {
                //TODO add verification
                timestamp_ = value;
            }
        }

        virtual protected void ValidateValue(Double value)
        {
        }

        private Sensor sensor_;
        private Double value_; //TODO isn't Double too big? Compare accuracy with sensor's parameters
        private DateTime timestamp_;
    }
}
