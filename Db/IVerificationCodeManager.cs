namespace Db
{
    public interface IVerificationCodeManager
    {
        int AddNewCode(string socialEmail);
        (bool, string) CheckVerificationCode(string socialEmail, int code);
    }
}