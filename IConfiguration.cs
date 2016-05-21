using System;
namespace meteostation
{
    public interface IConfiguration
    {
        string APIKey { get; }
        double MeasurementIntervalInMinutes { get; }
        string SensorIDForField(int fieldNumber);
    }
}
