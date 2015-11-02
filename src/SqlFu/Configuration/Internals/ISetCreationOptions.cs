namespace SqlFu.Configuration.Internals
{
    public interface ISetCreationOptions
    {
        void Throw();
        void DropIt();
        void Ignore();
    }
}