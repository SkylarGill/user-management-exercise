using System;

namespace UserManagement.Services.Exceptions;

public class UserMissingFromDataContextException : Exception
{
    public UserMissingFromDataContextException(long id) : base($"User with id '{id}' could not be found in the data context")
    {
    }
}