using System.Threading.Tasks;

namespace SqlFu
{
    public interface IQueryAndProcess
    {
        void Execute();
        Task ExecuteAsync();
    }
}