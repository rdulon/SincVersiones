using JumpsellerSync.Common.Util.Extensions;

namespace JumpsellerSync.DataAccess.Core
{
    public class DataAccessResult<TData> : DataAccessResult
    {
        public DataAccessResult(TData data, params string[] errors)
            : base(errors)
        {
            Data = data;
        }

        public TData Data { get; }
    }

    public class DataAccessResult
    {
        public DataAccessResult(params string[] errors)
        {
            Errors = errors ?? new string[0];
        }

        public string[] Errors { get; }

        public bool OperationSucceed => Errors.Length == 0;

        public static DataAccessResult Succeed() => new DataAccessResult();

        public static DataAccessResult<TData> Succeed<TData>(TData data = default)
            => new DataAccessResult<TData>(data);

        public static DataAccessResult Fail(params string[] errors)
            => new DataAccessResult(errors.ToFailureErrors());

        public static DataAccessResult<TData> Fail<TData>(
            TData data = default, params string[] errors)
                => new DataAccessResult<TData>(data, errors.ToFailureErrors());
    }
}
