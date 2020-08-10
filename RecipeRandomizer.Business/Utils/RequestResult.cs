using System.Collections.Generic;
using System.Net;

namespace RecipeRandomizer.Business.Utils
{
    public class RequestResult<T>
    {
        public T ReturnValue { get; set; }
        public List<string> Log { get; }
        public HttpStatusCode StatusCode { get; set; }

        public static implicit operator T(RequestResult<T> value)
        {
            return value.ReturnValue;
        }

        public static implicit operator RequestResult<T>(T value)
        {
            return new RequestResult<T> {ReturnValue = value};
        }

        public RequestResult(HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            Log = new List<string>();
            StatusCode = statusCode;
        }

        public void AddMessage(string message)
        {
            Log.Add(message);
        }

        public void AddMessages(IEnumerable<string> messages)
        {
            Log.AddRange(messages);
        }
    }
}
