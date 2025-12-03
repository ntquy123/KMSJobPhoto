using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entities.Common
{
    public class HandleList<Model>
    {
        public HandleList()
        {
        }
        public HandleList(IQueryable<Model> data)
        {
            if (data == null)
            {
                data = new List<Model>().AsQueryable();
            }
            this.total = data.AsEnumerable()?.LongCount();
            this.resultMsg = "success";
            this.data = data.AsEnumerable().AsQueryable();
            this.isSuccess = true;
            this.statusCode = 200;
        }
        public HandleList(List<Model> data)
        {
            if (data != null)
            {
                this.resultMsg = "success";
                this.data = data.AsQueryable();
                this.isSuccess = true;
                this.statusCode = 200;
            }
            else
            {
                this.resultMsg = "failed";
                this.data = new List<Model>().AsQueryable();
                this.isSuccess = false;
                this.statusCode = 404;
            }

        }
        public HandleList(bool isSuccess, string resultMsg)
        {
            this.total = 0;
            this.resultMsg = resultMsg;
            this.isSuccess = isSuccess;

        }
        public HandleList(int? total, string resultMsg, IQueryable<Model> data)
        {
            this.total = total;
            this.resultMsg = resultMsg;
            this.data = data;
        }
        public HandleList(int? total, bool isSuccess, IQueryable<Model> data)
        {
            this.total = total;
            this.resultMsg = isSuccess ? "Success" : "Failed";
            this.data = data;
            this.isSuccess = isSuccess;
        }
        public HandleList(int? page, int? pageSize, IQueryable<Model> query)
        {
            if (query == null)
            {
                query = new List<Model>().AsQueryable();
            }
            decimal total = query.AsEnumerable().LongCount();
            if (page > 0 && pageSize > 0)
            {
                int skip = (page.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }
            if (total > 0)
            {
                this.total = total;
                this.resultMsg = "Success";
                this.data = query;
                this.isSuccess = true;
            }
            else
            {
                this.total = total;
                this.resultMsg = "NO_DATA_FOUND";
                this.data = query;
                this.isSuccess = false;
            }

        }
        public HandleList(int? total, string resultMsg)
        {
            this.total = total;
            this.resultMsg = resultMsg;
        }
        public HandleList(int? total, string resultMsg, List<Model> data)
        {

            if (data == null)
            {
                data = new List<Model>();
            }
            this.total = total;
            this.resultMsg = resultMsg;
            this.data = data.AsQueryable();
            this.isSuccess = true;
        }

        public HandleList(long? total, bool isSuccess, IQueryable<Model> data)
        {
            this.total = total;
            this.resultMsg = isSuccess ? "Success" : "Failed";
            this.data = data;
            this.isSuccess = isSuccess;
        }
        public HandleList(decimal? total, bool isSuccess, IQueryable<Model> data)
        {
            this.total = total;
            this.resultMsg = isSuccess ? "Success" : "Failed";
            this.data = data;
            this.isSuccess = isSuccess;
        }

        public void SetSuccess(IQueryable<Model> data)
        {
            if (data == null)
            {
                data = new List<Model>().AsQueryable();
            }
            this.resultMsg = "success";
            this.data = data;
            this.isSuccess = true;
            this.statusCode = 200;
        }
        public void SetSuccess(Model data)
        {
            this.resultMsg = "success";
            this.data = (IQueryable<Model>)(IQueryable)data;
            this.isSuccess = true;
            this.statusCode = 200;
        }
        public void SetSuccess(List<Model> data)
        {
            if (data == null)
            {
                data = new List<Model>();
            }
            this.total = data.Count();
            this.resultMsg = "success";
            this.data = data.AsQueryable();
            this.isSuccess = true;
            this.statusCode = 200;
        }
        public void SetError(string strError)
        {
            this.total = null;
            this.resultMsg = strError;
            this.data = null;
            this.statusCode = 201;
            this.isSuccess = false;
        }
        public bool isSuccess { get; set; }
        public int statusCode { get; set; }
        public decimal? total { get; set; }
        public string resultMsg { get; set; }
        public IQueryable<Model> data { get; set; }
    }
}
