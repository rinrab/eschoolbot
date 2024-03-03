namespace ESchoolBot.Client
{
    [Serializable]
    public class LoginException : Exception
    {
        public LoginException() : base("Login error.")
        {
        }
    }
}