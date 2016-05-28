using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ThingSpeakWinRT;

using Newtonsoft.Json;

namespace WorkingTimePercentageCalculator
{
    class Program
    {
        // Classes to work with pairs were taken from here
        // http://stackoverflow.com/questions/577590/pair-wise-iteration-in-c-sharp-or-sliding-window-enumerator
        public class Pair<T, U>
        {
            public Pair()
            {
            }

            public Pair(T first, U second)
            {
                this.First = first;
                this.Second = second;
            }

            public T First { get; set; }
            public U Second { get; set; }
        };

        //TODO make a generic function?
        static Boolean DateTimeInRange(DateTime value, DateTime min, DateTime max)
        {
            return (min <= value) && (value < max);
        }

        static Boolean TimeSpanInRange(TimeSpan value, TimeSpan min, TimeSpan max)
        {
            return (min <= value) && (value < max);
        }

        static DateTime RemoveTime(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day);
        }

        static List<DateTime> GenerateLast30DaysList()
        {
            var result = new List<DateTime>();
            // We should not include today since data for today are incomplete
            DateTime minDate = RemoveTime(DateTime.Now.AddDays(-30));
            DateTime maxDate = RemoveTime(DateTime.Now);
            for (var dt = minDate; dt < maxDate; dt = dt.AddDays(1))
            {
                result.Add(RemoveTime(dt)); // Remove time here
            }
            return result;
        }

        public static IEnumerable<Pair<T, T>> Pairs<T>(IEnumerable<T> enumerable)
        {
            IEnumerator<T> e = enumerable.GetEnumerator(); e.MoveNext();
            T current = e.Current;
            while (e.MoveNext())
            {
                T next = e.Current;
                yield return new Pair<T, T>(current, next);
                current = next;
            }
        }

        static TimeSpan CalculateAverageIntervalForDay(DateTime day, ICollection<ThingSpeakFeed> dataForDay)
        {
            // Build a list of intervals between consequtive measurements
            var intervals = Pairs(dataForDay)
                .Select(pair => pair.Second.CreatedAt.Value - pair.First.CreatedAt.Value)
                // Filter out all intervals smaller or equal to 10 seconds
                .Where(diff => diff > TimeSpan.FromSeconds(10));

            Double averageIntervalInSeconds = intervals
                .Select(interval => interval.TotalSeconds)
                .Average();
            return TimeSpan.FromSeconds(averageIntervalInSeconds);
        }

        static TimeSpan GetManualIntervalForDay(DateTime day)
        {
            // Starting from April 20, 2016 I have 3 minute inverval. Before that, it was 1 minute.
            DateTime april20 = new DateTime(2016, 4, 20);
            if (day < april20)
            {
                return TimeSpan.FromMinutes(1);
            }
            else
            {
                return TimeSpan.FromMinutes(3);
            }
        }

        static void Main(string[] args)
        {
            var thingSpeakConnection = new ThingSpeakClient(sslRequired: true);
            Int32 channelId = 108891;

            List<DateTime> last_30days = GenerateLast30DaysList();

            // Unfortunately, ThingSpeak limits amount of data in one request to 8000, so we can't get all data in one huge request,
            // so we have to get data for each day in separate request (however, they can be parallelized)

            //TODO: parallelize calculation for different days
            for (var dt = last_30days.First(); dt < last_30days.Last(); dt = dt.AddDays(1))
            {
                // Step 1: retrieve data
                // TODO include other fields when data are available for them
                ThingSpeakData allDataForDay = thingSpeakConnection.ReadFieldsAsync(
                    String.Empty, channelId, fieldId: 1, start_date: dt, end_date: dt.AddDays(1)).Result;
                var dataForDay = allDataForDay.Feeds;

                Console.WriteLine("Retrieved {0} feeds for day {1}.", dataForDay.Count(), dt.ToString());

                Double workingTimeInPercents = 0.0;
                if (dataForDay.Count() >= 5)
                {
                    // We have enough data to calculate interval
                    // Step 2: find average interval between measurements
                    TimeSpan interval = GetManualIntervalForDay(dt);
                    Console.WriteLine("Found interval for day {0}, it is {1} minutes", dt.ToString(), interval.TotalMinutes);

                    Double intervalInSeconds = interval.TotalSeconds;
                    workingTimeInPercents = (dataForDay.Count() * intervalInSeconds / TimeSpan.FromDays(1).TotalSeconds) * 100.0;
                }

                Int32 roundedWorkingTimeInPercents = (Int32)Math.Round(workingTimeInPercents);
                Console.WriteLine("Working time percentage for day {0} is {1} %", dt.ToString(), roundedWorkingTimeInPercents);
            }

            Console.WriteLine("================================ Application finished. Press any key to exit =============================");
            Console.ReadKey(true);
        }
    }
}
