# This is a library to read sensors for Raspberry Pi implemented in C#

This repository contains a a bunch of useful stuff:
1. Library to read sensors (currently only DS18B20 are supported) for Raspberry Pi, implemented in C#. Library allows you to read measurements and do with them whatever you want.
2. Sensor reader application reads sensor data and sends them to ThingSpeak channel. ThingSpeak API key, measurement interval and sensor IDs are configurable using App.config (so you don't have to change the code to use it in your environment).
3. Working time percentage calculator, useful to determine stability of your weather station (or any other IoT device that sends info to ThingSpeak). **Currently it is not finished.**

Visual Studio solution definitely works with Visual Studio 2012 and 2013, should work with recent ones.

Library supports DS18B20 sensors only, but I may extend it in future (Though I don't give any guarantees. If you're interested in some particular sensor, please create an issue about that).

Library uses w1-therm kernel modules for low-level communication with sensors.

**Dependencies:**
* ThingSpeak application
   1. NLog
   2. NLog.Targets.Syslog (NLog extension)
   3. System.IO.Abstractions

* Sensor read library
    1. System.IO.Abstractions
* Unit tests
    1. NUnit 3
    2. NUnitLite
    3. System.IO.Abstractions
    4. System.IO.Abstractions.TestingHelpers

**Build instructions:**
1. Clone ThingSpeak library to your working directory.

Library is found here: https://github.com/DrHlus1/thingspeak-.net-library
2. Compile!
