using System;
using System.Collections.Generic;


namespace GroupMealApi.Models;

public class AccountScan
{
    public string? FirstName { get; set; } = null;

    public string? LastName { get; set; } = null;

    public string? Email { get; set; } = null;

    public string? PhoneNumber { get; set; } = null;

    public string? GroupId { get; set; }
}