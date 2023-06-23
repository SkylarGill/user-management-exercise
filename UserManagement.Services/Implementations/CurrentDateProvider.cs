using System;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Implementations;

public class CurrentDateProvider : ICurrentDateProvider
{
    public DateTime GetCurrentDate()
    {
        return DateTime.Now.Date;
    }
}