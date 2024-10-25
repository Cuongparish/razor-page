using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace razor.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int FailedLoginAttempts { get; set; }
        public bool IsLocked { get; set; } = false;
    }
}