﻿namespace CrudApi.Models
{
    public class User
    {
        public Guid Id { get; set; }
       
        public  string Email { get; set; }=string.Empty;
        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

    }
}
