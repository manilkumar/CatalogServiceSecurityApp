﻿namespace CatalogServiceSecurityApp.Services
{
    using CatalogServiceSecurityApp.Data;
    using CatalogServiceSecurityApp.Models.DbModels;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using CatalogServiceSecurityApp.Helpers;
    using BC = BCrypt.Net.BCrypt;
    using System.Security.Cryptography;

    public class AuthService
    {
        private readonly DataContext dataContext;
        private readonly IConfiguration configuration;

        public AuthService(DataContext dataContext, IConfiguration configuration)
        {
            this.dataContext = dataContext;
            this.configuration = configuration;
        }

        public bool IsAuthenticated(string email, string password)
        {
            var user = this.GetByEmail(email);
            return this.DoesUserExists(email) && BC.Verify(password, user.Password);
        }

        public bool DoesUserExists(string email)
        {
            var user = this.dataContext.Users.FirstOrDefault(x => x.Email == email);
            return user != null;
        }

        public User GetById(string id)
        {
            return this.dataContext.Users.FirstOrDefault(c => c.UserId == id);
        }

       
        public User GetByEmail(string email)
        {
            return this.dataContext.Users.FirstOrDefault(c => c.Email == email);
        }

        public User GetByUserName(string userName)
        {
            return this.dataContext.Users.FirstOrDefault(c => c.UserName == userName);
        }

        public User RegisterUser(User model)
        {
            var id = IdGenerator.CreateLetterId(10);
            var existWithId = this.GetById(id);
            while (existWithId != null)
            {
                id = IdGenerator.CreateLetterId(10);
                existWithId = this.GetById(id);
            }
            model.UserId = id;
            model.Password = BC.HashPassword(model.Password);

            var userEntity = this.dataContext.Users.Add(model);
            this.dataContext.SaveChanges();

            return userEntity.Entity;
        }

        public string GenerateJwtToken(string email, string role)
        {
            var issuer = this.configuration["Jwt:Issuer"];
            var audience = this.configuration["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(this.configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                            new Claim("Id", Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Sub, email),
                            new Claim(JwtRegisteredClaimNames.Email, email),
                            new Claim(ClaimTypes.Role, role),
                            new Claim(JwtRegisteredClaimNames.Jti,
                            Guid.NewGuid().ToString())
                        }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public string DecodeEmailFromToken(string token)
        {
            var decodedToken = new JwtSecurityTokenHandler();
            var indexOfTokenValue = 7;

            var t = decodedToken.ReadJwtToken(token.Substring(indexOfTokenValue));

            return t.Payload.FirstOrDefault(x => x.Key == "email").Value.ToString();
        }

        public User ChangeRole(string email, string role)
        {
            var user = this.GetByEmail(email);
            user.Role = role;
            this.dataContext.SaveChanges();


            return user;
        }
    }
}
