using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasCommerce.BuildingBlocks.Common.Results
{
    public class Result
    {
        protected internal Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None)
                throw new InvalidOperationException("Başarılı bir Result, hata bilgisi taşıyamaz.");

            if (!isSuccess && error == Error.None)
                throw new InvalidOperationException("Başarısız bir Result, mutlaka bir Error taşımalıdır.");

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get;}
        public bool IsFailure => !IsSuccess;
        public Error Error { get;}

        public static Result Success() => new(true, Error.None);
        public static Result Failure(Error error) => new(false, error);

        // Generic versiyonlara kısayol: bir değerle başarı döndürmek istediğinde Result<T>.Success(value) yazmak yerine bazen Result.Success(value) yazmak daha akıcı olur, o yüzden burada da tanımlıyoruz.
        public static Result<TValue> Success<TValue>(TValue value) => Result<TValue>.Success(value);
        public static Result<TValue> Failure<TValue>(Error error) => Result<TValue>.Failure(error);
    }

    public sealed class Result<TValue> : Result
    {
        private readonly TValue? _value;

        private Result(TValue? value) : base(true, Error.None)
        {
            _value = value;
        }

        private Result(Error error) : base(false, error)
        {
            _value = default;
        }

        public TValue Value => IsSuccess ? _value! 
            : throw new InvalidOperationException("Başarısız bir Result'ın değerine erişilemez.");

        public static Result<TValue> Success(TValue value) => new(value);
        public static new Result<TValue> Failure(Error error) => new(error);
    }
}
