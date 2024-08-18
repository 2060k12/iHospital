using System;
using System.Text.RegularExpressions;

public static class Validator
{
    // Regular expression for validating an email address
    private static readonly string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    public static bool IsValidEmail(string email, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            errorMessage = "Email cannot be empty.";
            return false;
        }

        if (!Regex.IsMatch(email, EmailPattern))
        {
            errorMessage = "Please enter a valid email address.";
            return false;
        }

        errorMessage = null;
        return true;
    }

    public static bool IsValidDate(string dateText, out string errorMessage)
    {
        // Define the range limits
        DateTime minDate = new DateTime(1960, 1, 1);
        DateTime maxDate = new DateTime(2024, 12, 31);

        if (string.IsNullOrWhiteSpace(dateText))
        {
            errorMessage = "Date cannot be empty.";
            return false;
        }

        if (!DateTime.TryParse(dateText, out DateTime parsedDate))
        {
            errorMessage = "Please enter a valid date.";
            return false;
        }

        if (parsedDate < minDate || parsedDate > maxDate)
        {
            errorMessage = $"Date must be between {minDate.ToShortDateString()} and {maxDate.ToShortDateString()}.";
            return false;
        }

        errorMessage = null;
        return true;
    }


    public static bool IsValidNumber(string numberText, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(numberText))
        {
            errorMessage = "Number cannot be empty.";
            return false;
        }

        if (!int.TryParse(numberText, out _))
        {
            errorMessage = "Please enter a valid number.";
            return false;
        }

        errorMessage = null;
        return true;
    }
}
