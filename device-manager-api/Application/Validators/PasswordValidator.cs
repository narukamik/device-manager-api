namespace device_manager_api.Application.Validators;

/// <summary>
/// Utility class for password validation
/// </summary>
public static class PasswordValidator
{
    /// <summary>
    /// Validates password meets complexity requirements:
    /// - Minimum 8 characters
    /// - At least 1 uppercase letter
    /// - At least 1 lowercase letter
    /// - At least 1 digit
    /// - At least 1 special character
    /// </summary>
    public static bool Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return false;

        var hasUpperCase = password.Any(char.IsUpper);
        var hasLowerCase = password.Any(char.IsLower);
        var hasDigit = password.Any(char.IsDigit);
        var hasSpecialChar = password.Any(c => "@$!%*?&#".Contains(c));

        return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
    }

    public static string GetRequirements()
    {
        return "Password must be at least 8 characters long and contain at least one uppercase letter, " +
               "one lowercase letter, one digit, and one special character (@$!%*?&#).";
    }
}
