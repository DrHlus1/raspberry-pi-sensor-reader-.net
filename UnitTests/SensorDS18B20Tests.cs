using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using meteostation;
using NUnit.Framework;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace UnitTests
{
    static class SensorDS18B20Tests
    {
        [Test]
        static public void LinuxFolderNameConstructorTest()
        {
            String linuxFolderName = "28-0000055f311a";
            Int64 fullID = 0x280000055f311a;
            var sensor = new SensorDS18B20(linuxFolderName);
            Assert.That(sensor.LinuxFolderName, Is.EqualTo(linuxFolderName));
            Assert.That(sensor.ID, Is.EqualTo(fullID));
        }

        [Test]
        static public void IDConstructorTest()
        {
            String linuxFolderName = "28-0000055f311a";
            Int64 fullID = 0x280000055f311a;
            var sensor = new SensorDS18B20(fullID);
            Assert.That(sensor.LinuxFolderName, Is.EqualTo(linuxFolderName));
            Assert.That(sensor.ID, Is.EqualTo(fullID));
        }

        [Test]
        static public void SuccessfullReadOfPositiveTemperatureTest()
        {
            String file_contents = "50 05 ff ff f0 ff ff ff 5c : crc=5c YES" + Environment.NewLine +
                                   "50 05 ff ff f0 ff ff ff 5c t=25123";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"/sys/bus/w1/devices/28-0000055f311a/w1_slave", new MockFileData(file_contents) }
            });
            var sensor = new SensorDS18B20("28-0000055f311a", fileSystem);
            List<Measurement> measurements = sensor.ReadMeasurements();
            Assert.That(measurements.Count, Is.EqualTo(1));
            DS18B20Measurement measurement = (DS18B20Measurement)measurements[0];
            Assert.That(measurement.Value, Is.EqualTo(25.123));
            //TODO add verification of timestamp
            Assert.That(measurement.Sensor, Is.EqualTo(sensor));
        }

        [Test]
        static public void SuccessfullReadOfZeroTemperatureTest()
        {
            //TODO this test case is almost the same as SuccessfullReadOfPositiveTemperatureTest. Avoid code duplication in some way
            String file_contents = "50 05 ff ff f0 ff ff ff 5c : crc=5c YES" + Environment.NewLine +
                                   "50 05 ff ff f0 ff ff ff 5c t=0";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"/sys/bus/w1/devices/28-0000055f311a/w1_slave", new MockFileData(file_contents) }
            });
            var sensor = new SensorDS18B20("28-0000055f311a", fileSystem);
            List<Measurement> measurements = sensor.ReadMeasurements();
            Assert.That(measurements.Count, Is.EqualTo(1));
            DS18B20Measurement measurement = (DS18B20Measurement)measurements[0];
            Assert.That(measurement.Value, Is.EqualTo(0));
            //TODO add verification of timestamp
            Assert.That(measurement.Sensor, Is.EqualTo(sensor));
        }

        [Test]
        static public void SuccessfullReadOfNegativeTemperatureTest()
        {
            //TODO this test case is almost the same as SuccessfullReadOfPositiveTemperatureTest. Avoid code duplication in some way
            String file_contents = "50 05 ff ff f0 ff ff ff 5c : crc=5c YES" + Environment.NewLine +
                                   "50 05 ff ff f0 ff ff ff 5c t=-10000";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"/sys/bus/w1/devices/28-0000055f311a/w1_slave", new MockFileData(file_contents) }
            });
            var sensor = new SensorDS18B20("28-0000055f311a", fileSystem);
            List<Measurement> measurements = sensor.ReadMeasurements();
            Assert.That(measurements.Count, Is.EqualTo(1));
            DS18B20Measurement measurement = (DS18B20Measurement)measurements[0];
            Assert.That(measurement.Value, Is.EqualTo(-10));
            //TODO add verification of timestamp
            Assert.That(measurement.Sensor, Is.EqualTo(sensor));
        }
    }

    static class DS18B20IdentifierTests
    {
        [Test]
        static public void NormalConstructorFromComponentsTest()
        {
            Byte family = 0x28;
            Int64 serialCode = 0x0000055f311a;
            var id = new DS18B20Identifier(family, serialCode);
            Assert.That(id.Family, Is.EqualTo(family));
            Assert.That(id.SerialCode, Is.EqualTo(serialCode));
        }

        [Test]
        static public void NormalConstructorFromFullIDTest()
        {
            Byte family = 0x28;
            Int64 serialCode = 0x0000055f311a;
            Int64 fullID = 0x280000055f311a;
            var id = new DS18B20Identifier(fullID);
            Assert.That(id.Family, Is.EqualTo(family));
            Assert.That(id.SerialCode, Is.EqualTo(serialCode));
        }

        [Test]
        static public void ToInt64Test()
        {
            Byte family = 0x28;
            Int64 serialCode = 0x0000055f311a;
            var id = new DS18B20Identifier(family, serialCode);
            Assert.That(id.ToInt64(), Is.EqualTo(0x280000055f311a));
        }
    }
}
