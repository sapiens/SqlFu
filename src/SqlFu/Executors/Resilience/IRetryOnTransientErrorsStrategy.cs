using System;

namespace SqlFu.Executors.Resilience
{
    public interface IRetryOnTransientErrorsStrategy
    {
        /// <summary>
        /// How much to wait before next try
        /// </summary>
        /// <returns></returns>
        TimeSpan GetWaitingPeriod();
        /// <summary>
        /// How many retries have been
        /// </summary>
        int RetriesCount { get; }

        /// <summary>
        /// If false, the db exception is rethrown
        /// </summary>
        bool CanRetry { get; }
    }
}