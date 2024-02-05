namespace ESchoolBot
{
    [Serializable]
    public class LoginException : Exception
    {
        public LoginException() : base("Login error.")
        {
        }
    }
}