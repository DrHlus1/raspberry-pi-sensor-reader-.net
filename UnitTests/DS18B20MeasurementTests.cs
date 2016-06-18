using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using meteostation;
using NUnit.Framework;

namespace UnitTests
{
    static class DS18B20MeasurementTests
    {
        [Test]
        public static void TempAsDoubleConstructorTest()
        {
            var sensor = new SensorDS18B20("28-0000055f311a");
            Double temp = 20.0;
            DateTime timestamp = DateTime.Now;
            var measurement = new DS18B20Measurement(sensor, temp, timestamp);
            Assert.That(measurement.Sensor, Is.EqualTo(sensor));
            Assert.That(measurement.Value, Is.EqualTo(temp));
            Assert.That(measurement.Timestamp, Is.EqualTo(timestamp));
        }

        [Test]
        public static void TempAsGoodFileContentsConstructorTest()
        {
            var sensor = new SensorDS18B20("28-0000055f311a");
            String file_contents = "50 05 ff ff f0 ff ff ff 5c : crc=5c YES" + Environment.NewLine +
                                   "50 05 ff ff f0 ff ff ff 5c t=25000";
            Double expected_temp = 25.0;
            DateTime timestamp = DateTime.Now;
            var measurement = new DS18B20Measurement(sensor, file_contents, timestamp);
            Assert.That(measurement.Sensor, Is.EqualTo(sensor));
            Assert.That(measurement.Value, Is.EqualTo(expected_temp));
            Assert.That(measurement.Timestamp, Is.EqualTo(timestamp));
        }

        //Verify that FormatException is thrown if null is given as file contents
        [Ignore("Functionality is not yet implemented")]
        [Test]
        public static void NullFileTest()
        {
            var sensor = new SensorDS18B20("28-0000055f311a");
            DateTime timestamp = DateTime.Now;
            Assert.That(() => new DS18B20Measurement(sensor: sensor, measurement: null, timestamp: timestamp), Throws.TypeOf<FormatException>());
        }

        //Verify that FormatException is thrown if given file contents is empty
        [Test]
        public static void EmptyFileTest()
        {
            var sensor = new SensorDS18B20("28-0000055f311a");
            DateTime timestamp = DateTime.Now;
            Assert.That(() => new DS18B20Measurement(sensor: sensor, measurement: String.Empty, timestamp: timestamp), Throws.TypeOf<FormatException>());
        }

        //Verify that FormatException is thrown if given file contents is 1 line long (should be 2 lines)
        [Test]
        public static void FileContents1LineTest()
        {
            var sensor = new SensorDS18B20("28-0000055f311a");
            DateTime timestamp = DateTime.Now;
            String file_contents = "50 05 ff ff f0 ff ff ff 5c : crc=5c YES";
            Assert.That(() => new DS18B20Measurement(sensor: sensor, measurement: file_contents, timestamp: timestamp), Throws.TypeOf<FormatException>());
        }

        //Verify that FormatException is thrown if given file contents is 3 lines long (should be 2 lines)
        [Test]
        public static void FileContents3LinesTest()
        {
            var sensor = new SensorDS18B20("28-0000055f311a");
            DateTime timestamp = DateTime.Now;
            String file_contents = "50 05 ff ff f0 ff ff ff 5c : crc=5c YES" + Environment.NewLine +
                                   "50 05 ff ff f0 ff ff ff 5c t=25000" + Environment.NewLine +
                                   "test";
            Assert.That(() => new DS18B20Measurement(sensor: sensor, measurement: file_contents, timestamp: timestamp), Throws.TypeOf<FormatException>());
        }

        // Verify that in case of corrupted file with measurement (where all values are zeros), exception is thrown
        [Test]
        public static void FileCorruptedWithZeroesTest()
        {
            var sensor = new SensorDS18B20("28-0000055f311a");
            DateTime timestamp = DateTime.Now;
            String file_contents = "00 00 00 00 00 00 00 00 00 : crc=00 YES" + Environment.NewLine +
                                   "00 00 00 00 00 00 00 00 00 t=0" + Environment.NewLine;
            Assert.That(() => new DS18B20Measurement(sensor: sensor, measurement: file_contents, timestamp: timestamp), Throws.TypeOf<FormatException>());
        }
    }
}
