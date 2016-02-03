namespace Tests.Data
{
    public class Address
    {
        public Address(string street)
        {
            Street = street;
        }

        public string Street { get; set; }
    }
}