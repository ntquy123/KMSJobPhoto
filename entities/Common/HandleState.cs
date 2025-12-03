using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entities.Common
{
    public class HandleState
    {

        public Dictionary<string, IEnumerable<string>> Errors;

        public HandleState()
        {
            Code = -99;
        }

        public HandleState(int code)
        {
            Code = code;
            GenCodeToMessage();
        }
        public HandleState(bool isSuccess)
        {
            Code = isSuccess ? 1 : 0;
            GenCodeToMessage();
        }
        public HandleState(bool isSuccess, string message)
        {
            Code = isSuccess ? 1 : 0;
            this.Message = message;
        }

        public HandleState(int code, string error)
        {
            this.Code = code;
            this.Message = error;
        }

        public HandleState(int v, Dictionary<string, IEnumerable<string>> errors)
        {
            this.Code = v;
            this.Errors = errors;
        }

        public HandleState(bool isSuccess, object data)
        {
            Code = isSuccess ? 1 : 0;
            GenCodeToMessage();
            this.Data = data;
        }

        public HandleState(bool isSuccess, string message, object data)
        {
            Code = isSuccess ? 1 : 0;
            this.Message = message;
            this.Data = data;
        }

        private void GenCodeToMessage()
        {
            switch (Code)
            {
                case 0:
                    Message = "False";
                    break;
                case 1:
                    Message = "True";
                    break;
                case 203:
                    Message = "OBJECT_EXISTED";
                    break;
                case 204:
                    Message = "OBJECT_NOT_EXISTED";
                    break;
            }
        }
        /// <summary>
        /// 0: False;
        /// 1: Success;
        /// 203: Object Existed
        /// 
        /// </summary>
        public int Code { get; set; }
        public bool isSuccess
        {
            get
            {
                return Code == 1;
            }
        }
        public string Message { get; set; }

        public object Data { get; set; }
    }
}
