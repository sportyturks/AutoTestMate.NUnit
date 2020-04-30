namespace AutoTestMate.NUnit.Services.Client
{
    public class ClientResponse
    {
        public virtual int Code { get; set; }
        public virtual string Status { get; set; }
        public virtual object Data { get; set; }
    }
}