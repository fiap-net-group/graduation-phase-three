using System.ComponentModel;

namespace TechBlog.NewsManager.API.Domain.Responses
{
    public enum ResponseMessage
    {
        [Description("An error ocurred, try again later")]
        GenericError = 0,
        [Description("Success")]
        Success = 1,
        [Description("User is not authenticated")]
        UserIsNotAuthenticated = 2,
        [Description("User already exists")]
        UserAlreadyExists = 4,
        [Description("Error creating user")]
        ErrorCreatingUser = 5,
        [Description("Invalid email")]
        InvalidEmail = 6,
        [Description("Invalid name")]
        InvalidName = 7,
        [Description("Invalid password")]
        InvalidPassword = 8,
        [Description("Invalid user type")]
        InvalidUserType = 9,
        [Description("Invalid information")]
        InvalidInformation = 10,
        [Description("Invalid credentials")]
        InvalidCredentials = 11,
        [Description("Invalid title")]
        InvalidTitle = 12,
        [Description("Invalid description")]
        InvalidDescription = 13,
        [Description("Invalid body")]
        InvalidBody = 14,
        [Description("Invalid tags")]
        InvalidTags = 15,
        [Description("User must be a journalist")]
        UserMustBeAJournalist = 16,
        [Description("Blog new not found")]
        BlogNewNotFound = 17
    }
}
