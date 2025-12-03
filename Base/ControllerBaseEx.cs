using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security;
using entities.Common;
using service.Common.Base.Interface;
using Microsoft.EntityFrameworkCore;
using erpsolution.api.Controllers.Base;
using erpsolution.entities.Common;
using erpsolution.lib;
using erpsolution.service.Common.Base.Interface;

namespace erpsolution.api.Base
{
[Route("[controller]")]
[ApiController]
public class ControllerBaseEx<IService, TModel, KeyType> : BaseController
        where IService : class
        where TModel : class, new()
{
    public readonly IService _service;
    protected DataModeType MODE_TYPE;
    protected ICurrentUser _currentUser;
    private IServiceBase<TModel> servicebase => (IServiceBase<TModel>)_service;
    //private readonly IMapper _mapper;
    private string PrimaryKey
    {
        get
        {
            return _service.GetType().GetProperties().FirstOrDefault(i => i.Name == nameof(PrimaryKey)).GetValue(_service).ToString();
        }
    }
    public ControllerBaseEx(IService service, ICurrentUser currentUser)
    {
        //_mapper = mapper;
        _currentUser = currentUser;
        _service = service;
        MODE_TYPE = DataModeType.View;
    }
    #region Basic
    /// <summary>
    /// Get List Model
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet]


    public virtual HandleList<TModel> Get(int? page = null, int? pageSize = null)
    {
        MODE_TYPE = DataModeType.View;

        var list = servicebase.Get();//ObjectUtil.ExecMethod(_service, "Get", null) as IQueryable<TModel>;

        return Paging(list, page, pageSize);
    }
    /// <summary>
    /// Get Model by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public virtual TModel First(KeyType id)
    {
        MODE_TYPE = DataModeType.View;
        var method = _service.GetType().GetMethod("First", new Type[] { typeof(Expression<Func<TModel, bool>>) });
        var item = method.Invoke(_service, new object[] { ObjectUtil.ExpressionBuilder<TModel, KeyType>(ExpressionType.Equal, PrimaryKey, id) });
        return item as TModel;
    }
    /// <summary>
    /// Add model
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(HandleResponse<object>), 400)]
    [ProducesResponseType(typeof(HandleResponse<object>), 200)]
    [AuthorizeEx(Permission.SAVE_YN)]

    public virtual IActionResult Add([FromBody] TModel item)
    {
        MODE_TYPE = DataModeType.Add;


        if (!ModelState.IsValid)
        {
            return GetResponse(ModelState);
            //   return Json(Error(ErrorCode.DATA_INVALID));
        }

        HandleState hs = ObjectUtil.ExecMethod(_service, "Add", item) as HandleState;
        return GetResponse(hs, item);

    }
    /// <summary>
    /// Update model
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    [HttpPut]
    [ProducesResponseType(typeof(HandleResponse<object>), 400)]
    [ProducesResponseType(typeof(HandleResponse<object>), 200)]
    [AuthorizeEx(Permission.SAVE_YN)]

    public virtual IActionResult Update([FromBody] TModel item)
    {
        MODE_TYPE = DataModeType.Update;


        if (!ModelState.IsValid)
        {

            return GetResponse(ModelState);
            //   return Json(Error(ErrorCode.DATA_INVALID));
        }

        var method = _service.GetType().GetMethod("Update", new Type[] { typeof(TModel), typeof(Expression<Func<TModel, bool>>) });
        HandleState hs = method.Invoke(_service, new object[] { item, ObjectUtil.ExpressionBuilder<TModel, KeyType>(ExpressionType.Equal, PrimaryKey, ObjectUtil.GetValue(item, PrimaryKey)) }) as HandleState;
        return GetResponse(hs, item);
    }
    /// <summary>
    /// Delete model
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(HandleResponse<object>), 400)]
    [ProducesResponseType(typeof(HandleResponse<object>), 200)]
    [AuthorizeEx(Permission.DELETE_YN)]
    public virtual IActionResult Delete(KeyType id)
    {
        MODE_TYPE = DataModeType.Delete;


        //SetAutoValue(item);
        var method = _service.GetType().GetMethod("Delete", new Type[] { typeof(Expression<Func<TModel, bool>>) });

        HandleState hs = method.Invoke(_service, new object[] { ObjectUtil.ExpressionBuilder<TModel, KeyType>(ExpressionType.Equal, PrimaryKey, id) }) as HandleState;
        return GetResponse(hs);
    }
    protected virtual IQueryable<TModel> PagingQuery(IQueryable<TModel> query, int? page = null, int? pageSize = null)
    {
        if (page > 0 && pageSize > 0)
        {
            int skip = (page.Value - 1) * pageSize.Value;
            query = query.Skip(skip).Take(pageSize.Value);
        }
        return query;
    }
    protected virtual HandleList<TModel> Paging(IQueryable<TModel> query, int? page = null, int? pageSize = null)
    {
        int total = query.Count();
        if (page > 0 && pageSize > 0)
        {
            int skip = (page.Value - 1) * pageSize.Value;
            query = query.Skip(skip).Take(pageSize.Value);
        }
        return new HandleList<TModel>(total, true, query);
    }
    protected virtual async Task<HandleList<TModel>> PagingAsync(IQueryable<TModel> query, int? page = null, int? pageSize = null)
    {
        {
            int total = await query.CountAsync();
            if (page > 0 && pageSize > 0)
            {
                int skip = (page.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }
            return new HandleList<TModel>(total, true, query);
        }
    }
    protected IActionResult GetResponse(ModelStateDictionary ModelState)
    {
        Dictionary<string, IEnumerable<string>> Errors = new Dictionary<string, IEnumerable<string>>();
        foreach (var state in ModelState)
        {
            Errors.Add(state.Key, state.Value.Errors.Select(i => (i.ErrorMessage.IndexOf(":") != -1 ? i.ErrorMessage.Split(":")[0].Replace(" ", "_").ToUpper() : i.ErrorMessage.Replace(" ", "_").ToUpper())));
        }
        return GetResponse(new HandleState(-99, Errors));
    }
    protected IActionResult GetResponse(HandleState hs, TModel model = null)
    {
        if (hs.isSuccess)
            return Ok(new HandleResponse<TModel>(hs.isSuccess, hs.Message, model, null));
        //Anontation
        if (hs.Code == -99) return Json(new HandleResponse<TModel>(hs.isSuccess, hs.Message, model, hs.Errors, hs.Code));
        return Json(new HandleResponse<TModel>(hs.isSuccess, hs.Message, model, null, hs.Code));
    }
    protected IActionResult GetResponse<T>(HandleList<T> hs)
    {
        if (hs.isSuccess)
            return Ok(new HandleResponse<IQueryable<T>>(hs.isSuccess, hs.resultMsg, hs.data, null));
        return Json(new HandleResponse<IQueryable<T>>(hs.isSuccess, hs.resultMsg, null, null, hs.statusCode));
    }

    protected IActionResult GetResponse(HandleState hs, object model)
    {
        if (hs.isSuccess)
            return Ok(new HandleResponse<object>(hs.isSuccess, hs.Message, model, null));
        //Anontation
        if (hs.Code == -99) return Json(new HandleResponse<object>(hs.isSuccess, hs.Message, model, hs.Errors, hs.Code));
        return Json(new HandleResponse<object>(hs.isSuccess, hs.Message, model, null, hs.Code));
    }
    #endregion
    #region Overwrite
    public override OkObjectResult Ok(object value)
    {
        return base.Ok(value);
    }
    public override BadRequestObjectResult BadRequest(object error)
    {
        return base.BadRequest(error);
    }
    public override JsonResult Json(object data)
    {
        return base.Json(data);
    }
    #endregion

    #region Advanded

    protected void AddAutoValue(DataFieldAutoValue dataFieldAutoValue)
    {
        servicebase.AddAutoValue(dataFieldAutoValue);
    }

    protected virtual HandleList<Object> Paging(IQueryable<Object> query, int? page = null, int? pageSize = null)
    {
        decimal total = (query != null && query.Count() > 0) ? query.LongCount() : 0;
        if (page > 0 && pageSize > 0)
        {
            int skip = (page.Value - 1) * pageSize.Value;
            query = query.Skip(skip).Take(pageSize.Value);
        }
        return new HandleList<Object>(total, true, query);
    }
    #endregion
}
}
