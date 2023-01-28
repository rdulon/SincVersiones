using JumpsellerSync.Common.Util.Extensions;

using System;

namespace JumpsellerSync.BusinessLogic.Core
{
    public enum ServiceResultStatus
    {
        Succeed = 0,
        BadInput,
        NotFound,
        UnAuthorized,
        Error,
        Exception
    }


    public class ServiceResult
    {
        private const string NOT_FOUND_DEFAULT = "El elemento no ha sido encontrado.";
        private const string BAD_INPUT_DEFAULT = "Los datos de entrada son incorrectos.";

        public ServiceResult(ServiceResultStatus status, params string[] errors)
        {
            errors ??= new string[0];
            if (status == ServiceResultStatus.Succeed && errors.Length > 0)
            { throw new ArgumentException(nameof(errors)); }

            Status = status;
            Errors = errors;
        }

        public ServiceResultStatus Status { get; }

        public string[] Errors { get; set; }

        public static ServiceResult Succeed()
            => new ServiceResult(ServiceResultStatus.Succeed);

        public static ServiceResult<TData> Succeed<TData>(TData data)
            => new ServiceResult<TData>(data, ServiceResultStatus.Succeed);

        public static ServiceResult BadInput(params string[] errors)
            => new ServiceResult(
                ServiceResultStatus.BadInput,
                errors.ToFailureErrors(BAD_INPUT_DEFAULT));

        public static ServiceResult<TData> BadInput<TData>(params string[] errors)
            => new ServiceResult<TData>(
                default,
                ServiceResultStatus.BadInput,
                errors.ToFailureErrors(BAD_INPUT_DEFAULT));

        public static ServiceResult NotFound(params string[] errors)
            => new ServiceResult(
                ServiceResultStatus.NotFound,
                errors.ToFailureErrors(NOT_FOUND_DEFAULT));

        public static ServiceResult<TData> NotFound<TData>(params string[] errors)
            => new ServiceResult<TData>(
                default,
                ServiceResultStatus.NotFound,
                errors.ToFailureErrors(NOT_FOUND_DEFAULT));

        public static ServiceResult Error(params string[] errors)
            => new ServiceResult(ServiceResultStatus.Error, errors.ToFailureErrors());

        public static ServiceResult<TData> Error<TData>(params string[] errors)
            => new ServiceResult<TData>(
                default, ServiceResultStatus.Error, errors.ToFailureErrors());
    }

    public class ServiceResult<TData> : ServiceResult
    {

        public ServiceResult(TData data, ServiceResultStatus status, params string[] errors)
            : base(status, errors)
        {
            Data = data;
        }

        public TData Data { get; }
    }
}
