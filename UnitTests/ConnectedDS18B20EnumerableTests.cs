using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Abstractions.TestingHelpers;

using NUnit.Framework;
using meteostation;

namespace UnitTests
{
    static class ConnectedDS18B20EnumerableTests
    {
        // Left for some other test case
        /*
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"/sys/bus/w1/devices/28-0000055f311a/w1_slave", new MockFileData("Testing is meh.") }
            });
             * */

        [Test]
        public static void ModuleNotLoadedTest()
        {
            //TODO by some reason sensors.GetEnumerator is not called when this test is compiled using VS 2012, so test fails
            var fileSystem = new MockFileSystem();
            var sensors = new ConnectedDS18B20Enumerable(fileSystem);
            Assert.That(() => { sensors.ToList(); }, Throws.TypeOf<OneWireModuleNotLoadedException>());
        }

        [Test]
        public static void NoSensorsConnectedTest()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"/sys/bus/w1/devices/this_file_should_not_be_here", new MockFileData("Testing is meh.") }
            });
            var enumerator = new ConnectedDS18B20Enumerable(fileSystem);
            var sensors = enumerator.ToList();
            Assert.That(sensors.Count, Is.EqualTo(0));
        }

        [Test]
        public static void OneSensorConnectedTest()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"/sys/bus/w1/devices/28-0000055f1020/w1_slave", new MockFileData("Testing is meh.") }
            });
            var enumerator = new ConnectedDS18B20Enumerable(fileSystem);
            var sensors = enumerator.ToList();
            Assert.That(sensors.Count, Is.EqualTo(1));
            Assert.That(sensors[0] is SensorDS18B20);
            var ds18b20_id = new DS18B20Identifier(0x28, 0x55f1020);
            Assert.That(sensors[0].ID, Is.EqualTo(ds18b20_id.ToInt64()));
        }

        [Test]
        public static void ManySensorsConnectedTest()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"/sys/bus/w1/devices/28-000000000001/w1_slave", new MockFileData("Testing is meh.") },
                { @"/sys/bus/w1/devices/28-000000000010/w1_slave", new MockFileData("Testing is meh.") },
                { @"/sys/bus/w1/devices/28-000000000100/w1_slave", new MockFileData("Testing is meh.") }
            });
            var ids = new DS18B20Identifier[] 
                { 
                    new DS18B20Identifier(0x28, 0x1),
                    new DS18B20Identifier(0x28, 0x10),
                    new DS18B20Identifier(0x28, 0x100)
                };
            var enumerator = new ConnectedDS18B20Enumerable(fileSystem);
            var sensors = enumerator.ToList();
            Assert.That(sensors.Count, Is.EqualTo(ids.Length));
            for (int i = 0; i < ids.Length; ++i)
            {
                Assert.That(sensors[i] is SensorDS18B20);
                Assert.That(sensors[i].ID, Is.EqualTo(ids[i].ToInt64()));
            }
        }
    }
}

