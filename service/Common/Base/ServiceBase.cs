using AutoMapper;
using entities.Common;
using erpsolution.dal.Context;
using erpsolution.dal.EF;
using erpsolution.entities.Common;
using erpsolution.service.Common.Base.Interface;
using Microsoft.EntityFrameworkCore;
using service.Common.Base.Interface;
using System.Data;
using System.Linq.Expressions;
namespace service.Common.Base
{
    public abstract class ServiceBase<Model> : IServiceBase<Model>
        where Model : class, new()
    {
        protected DataModeType MODE_TYPE;
        protected readonly AmtContext _amtContext;
        protected ICurrentUser _currentUser;
        protected readonly LogContext _logContext;
        private bool autoSaveChange = true;

        protected IMapper _mapper;
 
        public ServiceBase(IServiceProvider serviceProvider)
        {
            _currentUser = (ICurrentUser)serviceProvider.GetService(typeof(ICurrentUser));
            _mapper = (IMapper)serviceProvider.GetService(typeof(IMapper));
            _amtContext = (AmtContext)serviceProvider.GetService(typeof(AmtContext));
            _logContext = (LogContext)serviceProvider.GetService(typeof(LogContext));
        }
        private string modelName => typeof(Model).Name;

        public DbSet<Model> entities
        {
            get
            {
                return _amtContext.Set<Model>();
            }
        }

        public abstract string PrimaryKey { get; }

        //var a= _amtContext.Model.FindEntityType(typeof(Model)).FindPrimaryKey().Properties;
        #region Basic
        /// <summary>
        /// Get Data All
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<Model> Get()
        {

            return entities;
            //if (isSet)
            //    return entities;
            //else return queryEntities;
        }
        /// <summary>
        /// Get Data With Expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual IQueryable<Model> Get(Expression<Func<Model, bool>> expression)
        {
            return entities.Where(expression);
        }
        /// <summary>
        /// Get First with expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual Model First(Expression<Func<Model, bool>> expression)
        {
            return Get(expression).FirstOrDefault();
        }

        /// <summary>
        /// Get First with expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<Model> FirstAsync(Expression<Func<Model, bool>> expression)
        {
            return await Get(expression).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Add new Data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual HandleState Add(Model model)
        {
            MODE_TYPE = DataModeType.Add;
            //var checkUnique = CheckUnique(model, DataModeType.Add);
            //if (!checkUnique.isSuccess)
            //    return checkUnique;
            SetAutoValue(model);
            var valid = ValidateBeforeSave(model);
            if (!valid.isSuccess)
                return valid;

            entities.Add(model);
            return SaveChanges();
        }

        /// <summary>
        /// Add new Data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<HandleState> AddAsync(Model model)
        {
            MODE_TYPE = DataModeType.Add;
            SetAutoValue(model);
            var valid = ValidateBeforeSave(model);
            if (!valid.isSuccess)
            {
                return valid;
            }
            await entities.AddAsync(model);
            return await SaveChangesAsync();
        }

        public virtual HandleState AddMany(List<Model> lst)
        {
            MODE_TYPE = DataModeType.Add;
            foreach (Model model in lst)
            {
                SetAutoValue(model);
                var valid = ValidateBeforeSave(model);
                if (!valid.isSuccess)
                    return valid;

            };

            entities.AddRange(lst);
            return SaveChangesMany(lst.Count());
        }

        public virtual async Task<HandleState> AddManyAsync(List<Model> lst)
        {
            MODE_TYPE = DataModeType.Add;
            foreach (Model model in lst)
            {
                SetAutoValue(model);
                var valid = ValidateBeforeSave(model);
                if (!valid.isSuccess)
                    return valid;

            };
            await entities.AddRangeAsync(lst);
            return await SaveChangesManyAsync(lst.Count());
        }

        /// <summary>
        /// Update Data
        /// </summary>
        /// <param name="newModel"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual HandleState Update(Model newModel, Expression<Func<Model, bool>> expression)
        {
            Model currentModel = entities.FirstOrDefault(expression);
            if (currentModel != null)
            {
                MODE_TYPE = DataModeType.Update;
                SetAutoValue(newModel);
                var valid = ValidateBeforeSave(newModel);
                if (!valid.isSuccess)
                    return valid;
                _amtContext.Entry(currentModel).CurrentValues.SetValues(newModel);
                var track = _amtContext.Update(currentModel);
                return SaveChanges();
            }
            return new HandleState(204);
        }

        /// <summary>
        /// Update Data
        /// </summary>
        /// <param name="newModel"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<HandleState> UpdateAsync(Model newModel, Expression<Func<Model, bool>> expression)
        {
            Model currentModel = await entities.FirstOrDefaultAsync(expression);
            if (currentModel != null)
            {
                MODE_TYPE = DataModeType.Update;
                SetAutoValue(newModel);
                var valid = ValidateBeforeSave(newModel);
                if (!valid.isSuccess)
                    return valid;
                _amtContext.Entry(currentModel).CurrentValues.SetValues(newModel);
                var track = _amtContext.Update(currentModel);
                return await SaveChangesAsync();
            }
            return new HandleState(204);
        }

        public virtual HandleState UpdateMany(List<Model> newLst, Expression<Func<Model, bool>> expression)
        {
            var currentLst = entities.Where(expression);
            MODE_TYPE = DataModeType.Update;
            foreach (Model newModel in newLst)
            {
                SetAutoValue(newModel);
                var valid = ValidateBeforeSave(newModel);
                if (!valid.isSuccess)
                    return valid;
            }

            _amtContext.Entry(currentLst).CurrentValues.SetValues(newLst);
            _amtContext.UpdateRange(currentLst.AsEnumerable());
            return SaveChangesMany(newLst.Count());
        }
        /// <summary>
        /// Delete Data
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual HandleState Delete(Expression<Func<Model, bool>> expression)
        {
            MODE_TYPE = DataModeType.Delete;
            Model currentModel = entities.FirstOrDefault(expression);
            if (currentModel != null)
            {
                entities.Remove(currentModel);
                return SaveChanges();
            }
            return new HandleState(false, "DATA_NOT_FOUND");
        }
        public virtual async Task<HandleState> DeleteAsync(Expression<Func<Model, bool>> expression)
        {
            MODE_TYPE = DataModeType.Delete;
            Model currentModel = await entities.FirstOrDefaultAsync(expression);
            if (currentModel != null)
            {
                entities.Remove(currentModel);
                return await SaveChangesAsync();
            }
            return new HandleState(false, "DATA_NOT_FOUND");
        }

        public virtual HandleState DeleteMany(Expression<Func<Model, bool>> expression)
        {
            MODE_TYPE = DataModeType.Delete;
            var currentModel = entities.Where(expression);
            entities.RemoveRange(currentModel);
            return SaveChangesMany(currentModel.Count());
        }

        public virtual async Task<HandleState> DeleteManyAsync(Expression<Func<Model, bool>> expression)
        {
            MODE_TYPE = DataModeType.Delete;
            var currentModel = entities.Where(expression);
            entities.RemoveRange(currentModel);
            return await SaveChangesManyAsync(currentModel.Count());
        }

        public void SetAutoSave(bool autoSave)
        {
            autoSaveChange = autoSave;
        }
        private HandleState SaveChanges(int rowCount = 1)
        {
            try
            {
                if (autoSaveChange)
                {
                    int rowEffect = _amtContext.SaveChanges();
                    return new HandleState(true);
                }
                return new HandleState(true);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new HandleState(false, "DATA_INVALID");
            }
            catch (DbUpdateException ex)
            {
                //ORA-02292: integrity constraint (PK.R_21) violated - child record found => ORA-02292

                string messageRaw = ex.InnerException.Message;
                var lstMess = messageRaw.Split("\n");
                var Code = lstMess.Select(i => i.IndexOf(":") != -1 ? i.Split(":").FirstOrDefault() : i).FirstOrDefault();
                string message = Code != null ? Code.ToUpper() : "";
                return new HandleState(false, message);
            }

        }

        private async Task<HandleState> SaveChangesAsync(int rowCount = 1)
        {
            try
            {
                if (autoSaveChange)
                {
                    int rowEffect = await _amtContext.SaveChangesAsync();
                    return new HandleState(true);
                }
                return new HandleState(true);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new HandleState(false, "DATA_INVALID");
            }
            catch (DbUpdateException ex)
            {
                //ORA-02292: integrity constraint (PK.R_21) violated - child record found => ORA-02292
                string messageRaw = ex.InnerException.Message;
                var lstMess = messageRaw.Split("\n");
                var Code = lstMess.Select(i => i.IndexOf(":") != -1 ? i.Split(":").FirstOrDefault() : i).FirstOrDefault();
                string message = Code != null ? Code.ToUpper() : "";
                return new HandleState(false, message);
            }

        }

        private HandleState SaveChangesMany(int rowCount)
        {
            return SaveChanges(rowCount);

        }

        private async Task<HandleState> SaveChangesManyAsync(int rowCount)
        {
            return await SaveChangesAsync(rowCount);

        }

        #endregion
        #region Advanded
        private List<DataFieldAutoValue> _lAutoValue = new List<DataFieldAutoValue>();
        private List<DataColumnError> _lValidation = new List<DataColumnError>();
        /// <summary>
        /// when save data will check list here 
        /// example set user chager when update
        /// AddAutoValue(new DataFieldAutoValue(nameof(HrMasEmployee.Changer), UserCreator, dr =>  MODE_TYPE == DataModeType.Update));
        /// </summary>
        /// <param name="dataFieldAutoValue"></param>
        public void AddAutoValue(DataFieldAutoValue dataFieldAutoValue)
        {
            _lAutoValue.Add(dataFieldAutoValue);
        }
        protected void SetAutoValue(Model row)
        {
            if (row == null)
                return;

            foreach (var df in _lAutoValue)
                df.SetAutoValue(row);
        }
        /// <summary>
        /// Add validation when save data
        /// example
        ///  AddValidation(new DataColumnDynamicRulesError(nameof(HrMasEmployee.BirthYmd), "BirthYmd must has format YYYYMMdd", dr => {
        ///HrMasEmployee newItem = (HrMasEmployee)dr;
        ///        if (newItem.BirthYmd != null)
        ///        {
        ///            DateTime val = DateTime.Now;
        ///            return DateTime.TryParseExact(newItem.BirthYmd,"YYYYMMdd",null,System.Globalization.DateTimeStyles.None, out val);
        ///        }
        ///        return true;
        ///    }));
        /// </summary>
        /// <param name="validation"></param>
        protected void AddValidation(DataColumnError validation)
        {
            _lValidation.Add(validation);
        }
        protected HandleState ValidateBeforeSave(Model dr)
        {
            HandleValidation hv = new HandleValidation(dr);
            foreach (DataColumnError validation in _lValidation)
            {
                if (!validation.Validate(dr))
                {
                    if (validation.GetType() == typeof(DataColumnDynamicRulesError))
                    {
                        hv.AddError(validation._dataField, validation._error);
                    }
                    else
                    {
                        return new HandleState(205, validation._error);
                    }
                }
            }
            if (!hv.IsValid)
                return new HandleState(99, hv.Errors);
            return new HandleState(true);
        }
        //private Dictionary<List<string>, string> _uniqueFields = new Dictionary<List<string>, string>();
        //protected void Setunique(List<string> fields, string error = null)
        //{
        //    if (!_uniqueFields.ContainsKey(fields))
        //    {
        //        _uniqueFields.Add(fields, error);
        //    }
        //}
        //protected HandleState CheckUnique(Model entitty, DataModeType mode)

        //{
        //    foreach (var field in _uniqueFields)
        //    {
        //        if (mode == DataModeType.Update)
        //        {
        //            if (Exist(entitty, field.Key, PrimaryKey))
        //                return new HandleState(203);
        //        }
        //        else if (mode == DataModeType.Add)
        //        {
        //            if (Exist(entitty, field.Key))
        //                return new HandleState(203);
        //        }
        //    }
        //    return new HandleState(true);
        //}
        //private bool Exist(Model dr, List<string> KeyFields, string PrimaryKey = null)
        //{
        //    try
        //    {
        //        var pre = LinQUtil.True<Model>();
        //        var preC = LinQUtil.False<Model>();
        //        if (PrimaryKey != null)
        //            pre = pre.AndAlso(LinQUtil.NotEquals(dr, PrimaryKey));
        //        foreach (string KeyField in KeyFields)
        //            preC = preC.AndAlso(LinQUtil.GetLambda(dr, KeyField));
        //        pre = pre.AndAlso(preC);
        //        return _amtContext.Set<Model>().AnyAsync(pre).Result;
        //    }
        //    catch (Exception ex) { }
        //    return false;
        //}
        #endregion

        public string SpAutoGeneratedCode<T>(string defaultCode, int length, string keyColumn, string fieldColumn, string extraCondition, double newseed) where T : class
        {
            decimal seed = newseed == 0 ? 1 : ((decimal)newseed);
            var keyProperty = _amtContext.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.FirstOrDefault();
            var columnName = keyProperty?.GetColumnName();
            var tableName = GetTableName<T>(_amtContext);
            var sql = $"select (max({columnName}) + {seed}) from {tableName}";
            var result = _amtContext.ExcuteDataSet(sql);
            var firstRow = result.Tables[0].Rows[0];
            var finalCount = firstRow.ItemArray.FirstOrDefault();
            return finalCount.ToString();

        }
        public static string GetTableName<T>(DbContext context) where T : class
        {
            var models = context.Model;
            var entityTypes = models.GetEntityTypes();
            var entityTypeOfT = entityTypes.First(t => t.ClrType == typeof(T));
            var tableNameAnnotation = entityTypeOfT.GetAnnotation("Relational:TableName");
            var TableName = tableNameAnnotation.Value.ToString();

            return TableName;
        }


    }
}
