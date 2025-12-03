using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entities.Common
{
    public class HandleResponse<T>
    //where T:class
    {
        //When error
        public HandleResponse(string error)
        {
            this.message = error;
            this.success = false;
        }
        public HandleResponse(bool success, string message)
        {
            this.message = message;
            this.success = success;
        }
        public HandleResponse(bool success, string message, T data) : this(success, message)
        {
            this.data = data;
        }
        //When success
        public HandleResponse(T data)
        {
            if (data != null)
            {
                this.message = "success";
                this.data = data;
                this.success = true;
                this.statusCode = 200;
            }
            else
            {
                this.message = "failed";
                this.data = data;
                this.success = false;
                this.statusCode = 404;
            }

        }
        public HandleResponse(bool success, string message, T data, Dictionary<string, IEnumerable<string>> Errors = null, int StatusCode = 200)
        {
            this.success = success;
            this.message = message;
            this.data = data;
            this.Errors = Errors;
            this.statusCode = StatusCode;
        }

        public int statusCode { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public T data { get; set; }
        public Dictionary<string, IEnumerable<string>> Errors;
    }
}
