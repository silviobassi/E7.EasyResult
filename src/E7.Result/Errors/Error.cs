namespace E7.Result.Errors;

public class Error
{
    public string ErrorNature { get; set; }
    public string ErrorDescription { get; set; }
    
    public Error(string errorNature, string errorDescription)
    {
        ErrorNature = errorNature;
        ErrorDescription = errorDescription;
    }
    
    public override string ToString()
    {
        return "ErrorNature: " + ErrorNature + " ErrorDescription: " + ErrorDescription;
    }
}