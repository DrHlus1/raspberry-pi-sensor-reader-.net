using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace meteostation
{
    [Serializable]
    public class DS18B20Measurement: Measurement
    {
        public DS18B20Measurement(SensorDS18B20 sensor, Double temperature, DateTime timestamp)
            : base(sensor, temperature, timestamp)
        {
        }

        /*
         * This constructor takes string, read from file /sys/bus/w1/devices/<sensor ID>/w1_slave,
         * that contains measured data
         * If data are valid, measurement is initialized by contents of this string
         */
        public DS18B20Measurement(SensorDS18B20 sensor, String measurement, DateTime timestamp)
        {
            //TODO make it a constant or something like that
            String MessageFormatOfFormatException = "Received incorrect text data from sensor. {0}";
            //TODO verify sensor use 12-bit resolution
            /*
                 Example of w1_slave file contents in case of successful sensor read
                 
                 50 05 ff ff f0 ff ff ff 5c : crc=5c YES
                 50 05 ff ff f0 ff ff ff 5c t=85000
                 
                 First line ending with “YES” – which means no errors.
                 The second line ending with “t=” followed by the temperature in thousandths of degrees Celsius (°C * 1000)
                 
                 The power-on reset value of the temperature register is +85°C.
.
                */
            if (measurement == forbiddenMeasurement)
            {
                throw new FormatException(String.Format(MessageFormatOfFormatException, "Measurement file is corrupted."));
            }
            String[] lines = measurement.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length != 2)
            {
                throw new FormatException(String.Format(MessageFormatOfFormatException, "Line quantity is wrong."));
            }
            if (!lines[0].Contains("YES"))
            {
                throw new FormatException(String.Format(MessageFormatOfFormatException, "CRC checksum do not match."));
            }
            String temp_str = lines[1].Split(new string[] { "t=" }, StringSplitOptions.RemoveEmptyEntries)[1];
            //TODO number in text file is always an integer number. So using Int32.Parse may be a better alternative
            Double temp;
            if (!Double.TryParse(temp_str, out temp))
            {
                throw new FormatException(String.Format(MessageFormatOfFormatException, "Temperature is not in valid number format."));
            }
            Sensor = sensor;
            Value = temp / 1000;
            Timestamp = timestamp;
        }

        override protected void ValidateValue(Double value)
        {
            if (value == 85)
            {
                //TODO exception description
                throw new FormatException();
                // 85 degrees of Celsius is reserved value that is assigned to DS18B20 temperature register on start-up. 
                // I don't expect temperature higher that 50 degrees in my area, so consider this value as incorrect.
            }
        }

        public override string ToString()
        {
            return String.Format("{0} - {1} ℃", Sensor, Value);
        }

        // Measurement like this is possible to happen in case of errors, but it is never legal
        private String forbiddenMeasurement = 
            "00 00 00 00 00 00 00 00 00 : crc=00 YES" + Environment.NewLine +
            "00 00 00 00 00 00 00 00 00 t=0" + Environment.NewLine;
    }
}
