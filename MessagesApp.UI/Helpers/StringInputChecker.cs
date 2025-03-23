using System.Net.Mail;

namespace MessagesApp.UI.Helpers;

public class StringInputChecker
{
    public string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        string result =char.ToUpper(input[0]) + input.Substring(1);
        result = result.Trim();
        return result; 
    }
    
    public bool IsValidEmail(string input, out string formattedEmail)
    {
        formattedEmail = string.Empty;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        try
        {
            var email = new MailAddress(input);
            if (email.Address != input)
                return false;

            formattedEmail = email.Address.Trim().ToLower();
            return true;
        }
        catch
        {
            return false;
        }
    }
}