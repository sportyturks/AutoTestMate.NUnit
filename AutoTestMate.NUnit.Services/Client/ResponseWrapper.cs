namespace AutoTestMate.NUnit.Services.Client
{
    // ReSharper disable once UnusedMember.Global
    public class ResponseWrapper<T> : IResponseWrapper<T>

    {
        public virtual int Code { get; set; }

        public virtual string Status { get; set; }

        public virtual T Data { get; set; }
    }
}