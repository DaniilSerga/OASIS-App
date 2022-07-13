namespace OASIS_App.MVVM.Model
{
    public abstract class Person
    {
        protected static int _id;
        protected static string _login;
        protected static string _email;
        protected static string _mobilePhone;
        protected static string _password;

        public abstract int Id { get; set; }
        public abstract string Login { get; set; }
        public abstract string Email { get; set; }
        public abstract string MobilePhone { get; set; }
        public abstract string Password { get; set; }
    }
}
