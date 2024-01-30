using System.Text.Json.Serialization;

namespace TechBlog.NewsManager.API.Domain.Responses
{
    public class BaseResponseWithValue<T> : BaseResponse
    {
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T Value { get; set; }

        public BaseResponseWithValue<T> AsError(T value, ResponseMessage? message = null, params string[] errors)
        {
            AsError(message, errors);
            Value = value;
            return this;
        }

        public new BaseResponseWithValue<T> AsError(ResponseMessage? message = null, params string[] errors)
        {
            errors ??= Array.Empty<string>();
            base.AsError(message, errors);
            return this;
        }

        public BaseResponseWithValue<T> AsSuccess(T value)
        {
            AsSuccess();
            Value = value;
            return this;
        }
    }
}