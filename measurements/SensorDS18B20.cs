using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.IO.Abstractions;

namespace meteostation
{
    [Serializable]
    public class SensorDS18B20: Sensor
    {
        private readonly IFileSystem fileSystem_;

        public SensorDS18B20(Int64 id, String shortDescription = null, IFileSystem fileSystem = null)
        {
            ID = id;
            ShortDescription = shortDescription;

            if (fileSystem == null)
            {
                fileSystem = new FileSystem();
            }
            fileSystem_ = fileSystem;
        }

        public SensorDS18B20(String linuxFolderName, IFileSystem fileSystem = null)
        {
            LinuxFolderName = linuxFolderName;
            if (fileSystem == null)
            {
                fileSystem = new FileSystem();
            }
            fileSystem_ = fileSystem;
        }

        public String LinuxFolderName
        {
            get
            {
                return IDToLinuxFolderName(ID);
            }
            set
            {
                ID = LinuxFolderNameToID(value);
            }
        }

        public override List<Measurement> ReadMeasurements()
        {
            var result = new List<Measurement>(1);

            /* TODO use anynchronous mode to read files. Though files are really small,
            w1-therm driver sends request to measure temperate when application tries to read w1_slave file
            Data are returned ONLY when sensor finishes measurement.
            That means that using synchronous mode cause delay (t * N), where
              t - time, needed to measure temperature for one sensor (750 ms for 12-bit mode)
              N - number of sensors
             Asynchronous mode allows to send requests to all sensors at once, and then wait for them
             This reduces wait time to approx. 1 second in case of using 12-bit mode.
             More info you can find here
             http://raspberrypi.stackexchange.com/questions/14278/how-to-change-ds18b20-reading-resolution
             * */
            String file_contents = fileSystem_.File.ReadAllText(String.Format("/sys/bus/w1/devices/{0}/w1_slave", LinuxFolderName));
            var temperature = new DS18B20Measurement(this, file_contents, DateTime.Now);
            result.Add(temperature);
            return result;
        }

        protected override void ValidateID(Int64 id)
        {
            //TODO implement
        }

        static private Int64 LinuxFolderNameToID(String folderName)
        {
            String[] split_result = folderName.Split(new Char[] { '-' });
            Byte family_id = Byte.Parse(split_result[0], System.Globalization.NumberStyles.HexNumber);
            if (family_id != 0x28)
            {
                //TODO exception description
                throw new ArgumentException();
            }
            Int64 sensor_id = Int64.Parse(split_result[1], System.Globalization.NumberStyles.HexNumber);
            var id = new DS18B20Identifier(family_id, sensor_id);
            return id.ToInt64();
        }

        static private String IDToLinuxFolderName(Int64 id)
        {
            // Directory name looks like "28-0000055f311a"
            // 28 - family code for DS18B20
            // 00 00 05 5f 31 1a - unique sensor number
            var temp = new DS18B20Identifier(id);
            return String.Format("28-{0}", temp.SerialCode.ToString("x12"));
        }
    }
}