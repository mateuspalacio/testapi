﻿namespace Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsValid { get; set; }
        public int RequestLimit { get; set; } 
    }
}
