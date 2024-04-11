using Azure.Core;
using DevelWebApi.Modelos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DevelWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly string secretKey;

        public LoginController(IConfiguration config)
        {
            secretKey = config.GetSection("settings").GetSection("secretKey").ToString();
        }

        [HttpPost("Login", Name = "Login")]
        public dynamic Login(IConfiguration configuration, [FromBody] object datosLogin)
        {
            string resultado = string.Empty;
            string usuarioNombre = string.Empty;
            string contrasenia = string.Empty;

            try
            {
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Objects;

                var datos = JsonConvert.DeserializeObject<dynamic>(datosLogin.ToString());

                usuarioNombre = datos.usuario.ToString();
                contrasenia = datos.contrasenia.ToString();

                login usuario = AutenticacionLogin.GetUsuarios(configuration, usuarioNombre, contrasenia).FirstOrDefault();

                //if (usuario == null)
                //{
                //    return new
                //    {
                //        success = false,
                //        message = "No existe usuario con la información proporcionada",
                //        result = ""
                //    };
                //}

                //var jwt = configuration.GetSection("Jwt").Get<Jwt>();

                //var campos = new[]
                //{
                //    new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                //    new Claim("usuarioId", usuario.usuarioId.ToString()),
                //    new Claim("usuario", usuario.usuario)
                //};

                //var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
                //var inicioSesion =  new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

                //var token = new JwtSecurityToken(
                //    jwt.Issuer,
                //    jwt.Audience,
                //    campos,
                //    expires: DateTime.Now.AddDays(1)
                //    );

                var keyBytes = Encoding.ASCII.GetBytes(secretKey);
                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.usuario));
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.usuarioId.ToString()));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

                string tokencreado = tokenHandler.WriteToken(tokenConfig);

                return new
                {
                    succes = true,
                    message = $"Bienvenido {usuario.usuario}",
                    result = tokencreado
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Se ha generado un error al consultar usuarios: {ex.Message}");
            }
        }
    }
}
