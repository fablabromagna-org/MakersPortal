using System.Net;

namespace MakersPortal.Core.Exceptions.HttpExceptions
{
    public class InternalServerErrorException : BaseHttpException
    {
        public InternalServerErrorException()
        {
            StatusCode = HttpStatusCode.InternalServerError;
        }
    }
}