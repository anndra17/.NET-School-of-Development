using ReadingList.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default);
        bool HasChanges { get; }
    }
}
