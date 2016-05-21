using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Abstractions;

namespace meteostation
{
    public class ConnectedDS18B20Enumerable: IEnumerable<SensorDS18B20>
    {
        private readonly IFileSystem fileSystem_;

        public ConnectedDS18B20Enumerable(IFileSystem fileSystem)
        {
            fileSystem_ = fileSystem;
        }

        public ConnectedDS18B20Enumerable()
            : this (new FileSystem())
        {
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // uses the strongly typed IEnumerable<T> implementation
            return this.GetEnumerator();
        }

        // Normal implementation for IEnumerable<T>
        public IEnumerator<SensorDS18B20> GetEnumerator()
        {
            DirectoryInfoBase devicesDir = fileSystem_.DirectoryInfo.FromDirectoryName("/sys/bus/w1/devices");

            IEnumerable<DirectoryInfoBase> subfolders = null;
            try
            {
                subfolders = devicesDir.EnumerateDirectories("28*");
            }
            catch (DirectoryNotFoundException e)
            {
                throw new OneWireModuleNotLoadedException();
            }

            foreach (var deviceDir in subfolders)
            {
                //TODO it's strange to pass fileSystem in this way. Assign it using some Dependency Injection framework?
                yield return new SensorDS18B20(deviceDir.Name, fileSystem_);
            }
        }
    }
}
