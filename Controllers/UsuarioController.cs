using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DataMiner.Model;
using DataMiner.Model.Models;
using DataMinerBussiness.IBussiness;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace dataMinerMsForms.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class UsuarioController : ControllerBase
    {
        private IConfiguration _config;
        IUsuarioBussines usuarioBussines;
        public UsuarioController(IUsuarioBussines _usuarioBussines,IConfiguration config)
        {
            usuarioBussines = _usuarioBussines;
            _config = config;
        }
        [HttpGet]
        [Route("Login")]
       
        public IActionResult Login(string email, string password)
        {
            Response<UsuarioModel> user= usuarioBussines.Login(email, password);
            IActionResult response = Unauthorized();
            if (user.Code == ResponseEnum.Ok)
            {
                user.Result.us_athorization = GenerateJSONWebToken(user.Result);
                response = Ok(user);
            }
            return response;
            
        }
      
        private string GenerateJSONWebToken(UsuarioModel usuario)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Role, usuario.ro_nombre),
                new Claim(ClaimTypes.Name, usuario.us_nombre),
            });

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claimsIdentity.Claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();


            return jwtSecurityTokenHandler.WriteToken(token);
        }



    }
}