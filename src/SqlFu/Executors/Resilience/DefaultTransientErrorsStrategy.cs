using System;

namespace SqlFu.Executors.Resilience
{
    public class DefaultTransientErrorsStrategy : IRetryOnTransientErrorsStrategy, IConfigureDefaultTransientErrorsStrategy
    {
        /// <summary>
        /// How much to wait before attempting first retry. Default is 5 seconds.
        /// </summary>
        public TimeSpan FirstDelay { get; set; } = 5.ToSeconds();

        /// <summary>
        /// Function to calculate next delay between tries. By default is a linear progression increasing by 500ms.
        /// </summary>
        public Func<TimeSpan, TimeSpan> TimeDelayIncrement { get; set; } = prev => prev.Add(500.ToMiliseconds());

        /// <summary>
        /// default is max 3 retries
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        private TimeSpan _lastDelay;

        public TimeSpan GetWaitingPeriod()
        {
            RetriesCount++;
            if (RetriesCount == 1)
            {
                _lastDelay = FirstDelay;

            }
            else
            {
                _lastDelay = TimeDelayIncrement(_lastDelay);
            }
            return _lastDelay;
        }

        public int RetriesCount { get; private set; }
        public bool CanRetry => RetriesCount < MaxRetries;
    }
}