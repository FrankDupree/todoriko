using TodoServer.Entities.Interfaces;
using TodoServer.Entities;
using Microsoft.EntityFrameworkCore;


namespace TodoServer.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IRepository<Todo> _todoRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRepository<Todo> TodoRepository =>
            _todoRepository ??= new Repository<Todo>(_context);

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
