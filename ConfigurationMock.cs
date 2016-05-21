using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace meteostation
{
    public class ConfigurationMock: IConfiguration
    {
        public ConfigurationMock(String api_key, 
            Double measurementIntervalInMinutes, 
            Dictionary<Int32 /* Field number */, String /*Sensor ID*/> sensorIDs)
        {
            APIKey = api_key;
            MeasurementIntervalInMinutes = measurementIntervalInMinutes;
            SensorIDs_ = sensorIDs;
        }

        public String APIKey
        {
            get;
            private set;
        }

        public Double MeasurementIntervalInMinutes
        {
            get;
            private set;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="fieldNumber">Number of field. Should be between 1 and 8</param>
        public String SensorIDForField(int fieldNumber)
        {
            String result = null;
            SensorIDs_.TryGetValue(fieldNumber, out result);
            return result;
        }

        private Dictionary<Int32 /* Field number */, String /*Sensor ID*/> SensorIDs_;
    }
}
