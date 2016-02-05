using System;

namespace Tests.Mocks
{
    public class SomeData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }=DateTime.Now;
        public int Counter { get; set; }
    }
}