namespace dating_app_backend.src.Middlewares
{
    public class CustomException : Exception
    {
        public CustomException(string message) : base(message) { }

        // Override the ToString method to avoid serializing complex properties like TargetSite
        public override string ToString()
        {
            return $"{base.ToString()} - Method: {this.GetType().Name}";
        }
    }

}
