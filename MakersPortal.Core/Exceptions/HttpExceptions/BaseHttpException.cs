using System;
using System.Net;

namespace MakersPortal.Core.Exceptions.HttpExceptions
{
    public class BaseHttpException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        
        public object Value { get; set; }
    }
}