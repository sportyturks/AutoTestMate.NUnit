namespace AutoTestMate.NUnit.Services.Client
{
    public interface IResponseWrapper<T>

    {
        int Code { get; set; }

        string Status { get; set; }

        T Data { get; set; }
    }
}