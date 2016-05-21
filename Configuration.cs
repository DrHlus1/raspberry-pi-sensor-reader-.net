using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace meteostation
{
    public class Configuration : meteostation.IConfiguration
    {
        public String APIKey
        {
            get
            {
                return ConfigurationManager.AppSettings["APIKey"];
            }
        }

        public Double MeasurementIntervalInMinutes
        {
            get
            {
                String measurementIntervalSetting = ConfigurationManager.AppSettings["MeasurementIntervalInMinutes"];
                return Double.Parse(measurementIntervalSetting);
            }
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="fieldNumber">Number of field. Should be between 1 and 8</param>
        public String SensorIDForField(int fieldNumber)
        {
            if (fieldNumber < 1 || fieldNumber > 8)
            {
                throw new ArgumentOutOfRangeException("fieldNumber", "Field number is out of range of acceptable values (should be between 1 and 8)");
            }
            return ConfigurationManager.AppSettings["SensorIDForField" + fieldNumber.ToString()];
        }
    }
}
