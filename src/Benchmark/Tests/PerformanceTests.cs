using CavemanTools.Testing;

namespace Benchmark.Tests
{
    public abstract class PerformanceTests
    {
        public abstract void FetchSingleEntity(BenchmarksContainer bc);
        public abstract void FetchSingleDynamicEntity(BenchmarksContainer bc);
        public abstract void QueryTop10(BenchmarksContainer bc);
        public abstract void QueryTop10Dynamic(BenchmarksContainer bc);
        public abstract void PagedQuery_Skip0_Take10(BenchmarksContainer bc);
        public abstract void ExecuteScalar(BenchmarksContainer bc);
        public abstract void MultiPocoMapping(BenchmarksContainer bc);
        public abstract void Inserts(BenchmarksContainer bc);
        public abstract void Updates(BenchmarksContainer bc);

    }
}