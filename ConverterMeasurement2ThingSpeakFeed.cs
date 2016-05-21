using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThingSpeakWinRT;
using meteostation;

using System.Reflection;

namespace meteostation
{
    static public class ConverterMeasurement2ThingSpeakFeed
    {
        static public ThingSpeakFeed Convert(List<Measurement> measurements, IConfiguration config)
        {
            var result = new ThingSpeakFeed();
            var field_numbers = new Dictionary<String /*Sensor ID*/, int /*Field number*/>();
            for (int field_number = 1; field_number <= 8; ++field_number)
            {
                String sensor_id = config.SensorIDForField(field_number);
                if (!String.IsNullOrEmpty(sensor_id))
                {
                    field_numbers.Add(sensor_id, field_number);
                }
            }
            
            foreach (DS18B20Measurement measurement in measurements)
            {
                String sensor_id = ((SensorDS18B20)measurement.Sensor).LinuxFolderName;
                int field_number = 0;
                if (field_numbers.TryGetValue(sensor_id, out field_number))
                {
                    if (field_number >= 1 && field_number <= 8)
                    {
                        typeof(ThingSpeakFeed).GetTypeInfo().GetDeclaredProperty("Field" + field_number).
                            SetValue(result, measurement.Value.ToString());
                    }
                }
            }
            // Find average timestamp to find deviation
            Double avg_timestamp_ticks = measurements.Select(m => m.Timestamp.Ticks).Average();
            if (measurements.Any(m => Math.Abs(m.Timestamp.Ticks - avg_timestamp_ticks) >= TimeSpan.TicksPerSecond))
            {
                // TODO warning about deviation in timestamp
            }
            result.CreatedAt = new DateTime((Int64)avg_timestamp_ticks);
            return result;
        }
    }
}
