namespace TodoServer.Entities.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Todo> TodoRepository { get; }
        Task<int> CompleteAsync();
    }
}
