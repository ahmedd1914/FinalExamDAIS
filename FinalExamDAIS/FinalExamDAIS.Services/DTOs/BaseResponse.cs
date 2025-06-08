using System.Collections.Generic;
using System.Linq;

namespace FinalExamDAIS.Services.DTOs
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class BaseResponse<T> : BaseResponse
    {
        public T Data { get; set; }
    }
} 