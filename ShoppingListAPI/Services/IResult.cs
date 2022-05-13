using System;
using System.Collections.Generic;

namespace ShoppingListAPI.Services
{
    public interface IResult
    {
        public int StatusCode { get; }

        public bool IsSuccess { get; }

        public bool IsNotFound { get; }

        public object Value { get; }

        public ICollection<(string, string)> Errors { get; }
    }

    public class Result : IResult
    {
        public int StatusCode { get; private set; }

        public bool IsSuccess => StatusCode >= 200 && StatusCode <= 299;

        public bool IsNotFound => StatusCode == 404;

        protected readonly object _Value;
        public object Value
        {
            get
            {
                if (IsSuccess)
                    return _Value;
                throw new InvalidOperationException();
            }
        }

        public ICollection<(string, string)> Errors { get; } = new List<(string, string)>();

        protected Result(int statusCode, object value = null)
        {
            StatusCode = statusCode;
            _Value = value;
        }

        // Static Constructors
        public static Result Ok() => new(200);
        public static Result<T> Ok<T>(T value) => new(200, value);

        public static Result Created() => new(201);
        public static Result<T> Created<T>(T value) => new(201, value);

        public static Result BadRequest() => new(400);
        public static Result BadRequest(string key, string error)
        {
            var result = new Result(400);
            result.Errors.Add((key, error));
            return result;
        }

        public static Result Unauthorized() => new(401);

        public static Result Forbidden() => new(403);

        public static Result NotFound() => new(404);

        public static Result ServerError() => new(500);
    }

    public class Result<T> : Result
    {
        public new T Value => (T)_Value;

        internal Result(int statusCode, T value = default) : base(statusCode, value)
        {
        }
    }
}
