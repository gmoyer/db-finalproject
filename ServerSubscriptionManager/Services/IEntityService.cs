using ServerSubscriptionManager.Models;

namespace ServerSubscriptionManager.Services
{
    public interface IEntityService<T>
    {
        public Task<bool> AddAsync(T entity);
        public Task<bool> RemoveAsync(long id);
        public Task<bool> UpdateAsync(T entity);
    }
}
