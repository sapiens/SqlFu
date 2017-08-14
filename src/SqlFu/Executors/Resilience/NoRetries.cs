using System;

namespace SqlFu.Executors.Resilience
{
    public class NoRetries : IRetryOnTransientErrorsStrategy
    {
        public TimeSpan GetWaitingPeriod()
        {
            throw new NotImplementedException();
        }

        public int RetriesCount { get; }
        public bool CanRetry => false;
    }
}