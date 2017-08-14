using System;

namespace SqlFu.Executors.Resilience
{
    public interface IConfigureDefaultTransientErrorsStrategy
    {
        /// <summary>
        /// How much to wait before attempting first retry. Default is 5 seconds.
        /// </summary>
        TimeSpan FirstDelay { get; set; }

        /// <summary>
        /// Function to calculate next delay between tries. By default is a linear progression increasing by 500ms.
        /// </summary>
        Func<TimeSpan, TimeSpan> TimeDelayIncrement { get; set; }

        /// <summary>
        /// default is max 3 retries
        /// </summary>
        int MaxRetries { get; set; }
    }
}