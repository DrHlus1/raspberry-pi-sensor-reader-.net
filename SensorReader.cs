using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Abstractions;

using NLog;
using meteostation;

using System.Net;

using ThingSpeakWinRT;

namespace meteostation
{
    class SensorReader
    {
        public void Work(IFileSystem fileSystem, IConfiguration config)
        {
            ReadAndSendMeasurementsPeriodically(fileSystem, config);
        }

        public void Work()
        {
            Work(new FileSystem(), new Configuration());
        }

        void ReadAndSendMeasurements(IFileSystem fileSystem, IConfiguration config)
        {
            try
            {
                logger_.Info("Starting another measurement");
                var conn_sensors_enumerable = new ConnectedDS18B20Enumerable(fileSystem);
                List<SensorDS18B20> connected_sensors = conn_sensors_enumerable.ToList();
                var measurements = new List<Measurement>();
                foreach (var sensor in connected_sensors)
                {
                    List<Measurement> sensor_measurements = sensor.ReadMeasurements();
                    measurements.AddRange(sensor_measurements);
                    foreach (var measurement in sensor_measurements)
                    {
                        logger_.Info(String.Format("Retrieved measurement from sensor ID {0}: temperature {1} °C", sensor.LinuxFolderName, measurement.Value));
                    }
                }
                logger_.Info("Measurements are ready to be sent");

                try
                {
                    ThingSpeakFeed feed = ConverterMeasurement2ThingSpeakFeed.Convert(measurements, config);
                    var thingspeak_conn = new ThingSpeakClient(sslRequired: true);
                    thingspeak_conn.UpdateFeedAsync(config.APIKey, feed).Wait();

                    logger_.Info("Measurement is finished");
                }
                catch (System.Net.WebException ex)
                {
                    logger_.Error("Failed to send measurements to server: " + ex.ToString());
                }
            }
            catch (OneWireModuleNotLoadedException e)
            {
                logger_.Error("1-Wire module is not loaded. Unfortunately, author of this app is lazy and did not implement feature to load required modules.");
                throw;
            }
            catch (Exception e)
            {
                logger_.Error("Reading measurement failed, caught exception: " + e.ToString());
                //throw; //TODO really we should let app die since we may have memory or stack corruption or other horrible things going
                // But since it's complicated to restart the app without systemd, that line is commented for now.
                // It will be uncommented when systemd service is implememted
            }
        }

        void ReadAndSendMeasurementsPeriodically(IFileSystem fileSystem, IConfiguration config)
        {
            new Timer((Object) => ReadAndSendMeasurements(fileSystem, config), null, new TimeSpan(0), TimeSpan.FromMinutes(config.MeasurementIntervalInMinutes));
            Thread.Sleep(Timeout.InfiniteTimeSpan);
        }

        private Logger logger_ = LogManager.GetCurrentClassLogger();
    }
}
