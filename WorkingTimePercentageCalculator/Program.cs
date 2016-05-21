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

        static List<DateTime> GenerateLast30DaysList()
        {
            var result = new List<DateTime>();
            // We should not include today since data for today are incomplete
            DateTime minDate = DateTime.Now.AddDays(-30);
            minDate = new DateTime(minDate.Year, minDate.Month, minDate.Day); // Remove time from date to improve working on recent days
            DateTime maxDate = DateTime.Now;
            for (var dt = minDate; dt < maxDate; dt = dt.AddDays(1))
            {
                result.Add(new DateTime(dt.Year, dt.Month, dt.Day)); // Remove time here
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

        static void Main(string[] args)
        {
            var thingSpeakConnection = new ThingSpeakClient(sslRequired: true);
            Int32 channelId = 108891;
            // Step 1: retrieve data for last 30 days
            // TODO we need measurements for only last 30 days. Stop downloading all of them
            // TODO include other fields when data are available for them
            ThingSpeakData data = thingSpeakConnection.ReadFieldsAsync(String.Empty, channelId, 1).Result;

            List<DateTime> last_30days = GenerateLast30DaysList();
            var dataForLast30Days = data.Feeds.Where(feed => DateTimeInRange(
                feed.CreatedAt.Value, last_30days.First(), last_30days.Last()));
            
            
            for (var dt = last_30days.First(); dt < last_30days.Last(); dt = dt.AddDays(1))
            {
                // Step 2: find average interval between measurements
                TimeSpan averageInterval = TimeSpan.Zero;

                var dataForDay = dataForLast30Days.Where(feed => DateTimeInRange(
                    feed.CreatedAt.Value, dt, dt.AddDays(1)));

                Console.WriteLine("Retrieved {0} feeds for day {1}.", dataForDay.Count(), dt.ToString());

                if (dataForDay.Count() < 5)
                {
                    continue;
                }
                // We have enough data to calculate interval

                // Build a list of intervals between consequtive measurements
                var intervals = Pairs(dataForDay)
                    .Select(pair => pair.Second.CreatedAt.Value - pair.First.CreatedAt.Value)
                    // Filter all intervals smaller than 1 minute and larger than 10 minutes
                    .Where(diff => TimeSpanInRange(diff, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(10)));
                Int32 interval_count = intervals.Count();
                Double averageIntervalInMinutes = intervals.Aggregate<TimeSpan, Double>(
                    0.0,
                    (average, interval) => (average + interval.TotalMinutes / interval_count));
                averageInterval = TimeSpan.FromMinutes(averageIntervalInMinutes);
                Console.WriteLine("Found average interval for day {0}, interval is {1} minutes", dt.ToString(), averageInterval.TotalMinutes);

                Double workingTimeInPercents = (dataForDay.Count() * averageIntervalInMinutes / TimeSpan.FromDays(1).TotalMinutes) * 100.0;
                Console.WriteLine("Working time percentage for day {0} is {1} %", dt.ToString(), workingTimeInPercents);
            }

            /*
            foreach (var feed in dataForLast30Days)
            {
                Console.WriteLine(JsonConvert.SerializeObject(feed));
            }
             * */
            Console.WriteLine("================================ Application finished. Press any key to exit =============================");
            Console.ReadKey(true);
        }
    }
}
