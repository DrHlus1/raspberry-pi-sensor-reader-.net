using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ThingSpeakWinRT;
using meteostation;

namespace UnitTests
{
    class ConverterMeasurement2ThingSpeakFeedTests
    {
        [Test]
        static public void OneFieldTest()
        {
            String sensor_id = "28-0000055f1020";
            var sensor = new SensorDS18B20(sensor_id);
            Double temperature = 36.6;
            DateTime timestamp = DateTime.Now;
            var measurement = new DS18B20Measurement(sensor, temperature, timestamp);
            var config = new ConfigurationMock("API key", 1, new Dictionary<Int32, String> { { 1, sensor_id } } );
            var measurement_list = new List<Measurement> { measurement };
            ThingSpeakFeed result = ConverterMeasurement2ThingSpeakFeed.Convert(measurement_list, config);
            // First field should contain temperature
            Assert.That(result.Field1, Is.EqualTo(temperature.ToString()));
            // Other fields should be empty
            // TODO: add a helper function to make an array instead of all there FieldN properties?
            Assert.That(result.Field2, Is.Null);
            Assert.That(result.Field3, Is.Null);
            Assert.That(result.Field4, Is.Null);
            Assert.That(result.Field5, Is.Null);
            Assert.That(result.Field6, Is.Null);
            Assert.That(result.Field7, Is.Null);
            Assert.That(result.Field8, Is.Null);
            Assert.That(result.CreatedAt, Is.EqualTo(timestamp).Within(1).Seconds);
            //TODO add verification for other fields if needed
        }
    }
}
