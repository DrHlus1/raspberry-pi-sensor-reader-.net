This repository contains a library and application to read sensors (currently only DS18B20 are supported) for Raspberry Pi implemented in C#.

Library allows you to read measurements and do with them wherever you want. Application reads sensor data and sends them to ThingSpeak channel. ThingSpeak API key, measurement interval and sensor IDs are configurable using App.config (so you don't have to change the code to use it on your environment).

Library supports DS18B20 sensors only, but I may extend it in future (Though I don't give any guarantees. If you're interested in some particular sensor, please create an issue about that).

Library uses w1-therm kernel modules for low-level communication with sensors.

