using System.Collections.Generic;
using System.Threading.Tasks;
using erpsolution.dal.EF;
using erpsolution.entities;

namespace erpsolution.service.Interface
{
    public interface IAmtTodoService
    {
        new string PrimaryKey { get; }

        Task<List<AuditTodoRow>> GetTodoListAsync(AmtTodoRequest request);
    }
}
