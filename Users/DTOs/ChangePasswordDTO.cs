using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ChangePasswordDTO
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
