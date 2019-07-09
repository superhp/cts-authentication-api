namespace Communication
{
    public interface IEmailManager
    {
        void SendVerificationCode(string emailAddress, int code);
    }
}