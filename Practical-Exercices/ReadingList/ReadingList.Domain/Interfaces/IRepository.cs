using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Domain.Interfaces
{
    public interface IRepository<T, TKey>
        where TKey : notnull
        where T : class
    {
        bool Add(T entity);
        bool Upsert(T entity);
        bool Remove(TKey id);
        bool TryGet(TKey id, out T? entity);
        IReadOnlyCollection<T> GetAll();

        int Count {  get; }
    }
}
