using entities.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace service.Common.Base.Interface
{
    public interface IServiceBase<Model> where Model : class, new()
    {
        DbSet<Model> entities { get; }
        string PrimaryKey { get; }
        //DbQuery<Model> queryEntities { get; }

        HandleState Add(Model model);
        HandleState AddMany(List<Model> lst);
        HandleState Delete(Expression<Func<Model, bool>> expression);
        HandleState DeleteMany(Expression<Func<Model, bool>> expression);
        Model First(Expression<Func<Model, bool>> expression);
        IQueryable<Model> Get();
        IQueryable<Model> Get(Expression<Func<Model, bool>> expression);
        HandleState Update(Model newModel, Expression<Func<Model, bool>> expression);
        //HandleState UpdateMany(List<Model> newLst, Expression<Func<Model, bool>> expression);
        void AddAutoValue(DataFieldAutoValue dataFieldAutoValue);
        void SetAutoSave(bool autoSave);

        Task<HandleState> AddAsync(Model model);
        Task<HandleState> AddManyAsync(List<Model> lst);
        Task<Model> FirstAsync(Expression<Func<Model, bool>> expression);
        Task<HandleState> UpdateAsync(Model newModel, Expression<Func<Model, bool>> expression);

        Task<HandleState> DeleteAsync(Expression<Func<Model, bool>> expression);
        Task<HandleState> DeleteManyAsync(Expression<Func<Model, bool>> expression);
    }
}
