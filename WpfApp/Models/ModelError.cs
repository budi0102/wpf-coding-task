namespace WpfApp.Models
{
    public class ModelError
    {
        public ModelError(string propertyName, string errorMessage)
        {
            this.PropertyName = propertyName;
            this.ErrorMessage = errorMessage;
        }

        public string PropertyName { get; }
        public string ErrorMessage { get; }
    }
}
