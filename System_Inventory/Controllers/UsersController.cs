using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System_Inventory.ModelsSystemInventory;

namespace System_Inventory.Controllers
{
    [Route("Users")]
    [ApiController]
    public class UsersController : Controller
    {
        private IConfiguration _configuration;
        public UsersController(IConfiguration configuration) { 
            _configuration = configuration;
        }

        [HttpPost]
        [Route("/Login")]
        public IActionResult LoginUser([FromBody]UserCredentials userCredentials)
        {
            try
            {
                using(InventorySystemContext db  = new InventorySystemContext())
                {
                    User user = db.Users.Where( u => u.UserName == userCredentials.UserName).FirstOrDefault();
                    if (user == null) 
                    {
                        return StatusCode(StatusCodes.Status404NotFound, new { message = "Usuario no encontrado." });
                    }
                    else
                    {
                        if(user.KeyPassword == userCredentials.Password) {

                            //Configuracion del token                       
                            var jwtOption = _configuration.GetSection("Jwt").Get<JwtSeccion>();

                            //Almacenamiento en Token
                            var claims = new[]
                            {
                                new Claim(JwtRegisteredClaimNames.Sub,jwtOption.Subject),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
                                new Claim("UserId",user.UserId.ToString())
                            };
                           
                            //Obtener la llave
                            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.Key));
                            //Inicio sesion
                            var singIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                            //Creat token
                            var token = new JwtSecurityToken(
                                jwtOption.Issuer,
                                jwtOption.Audience,
                                claims,
                                expires: DateTime.Now.AddDays(2),
                                signingCredentials: singIn
                                );
                            return StatusCode(StatusCodes.Status202Accepted, new { message = "Acceso realizado.", UsuarioID = user.UserId, Token = new JwtSecurityTokenHandler().WriteToken(token) });
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status401Unauthorized, new { message = "La contraseña no es correcta." });
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error: {ex}" });
            }
        }

        [HttpPost]
        [Route("Create_User")]
        public IActionResult CreateUser([FromBody] UserCreateCredentials userCreateCredentials)
        {
            if (userCreateCredentials.Name == string.Empty)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = "Atención: Debe agregar un nombre como mínimo." });
            }
            else if (userCreateCredentials.Credential.Length < 14)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = "Atención: La cédula debe ir sin espacios y tener 14 caracteres." });
            }
            try
            {
                string[] partesNombre = userCreateCredentials.Name.Split();
                string partesCombinadas = $"{string.Concat(partesNombre.Select(parte => parte[0])).ToLower()}{userCreateCredentials.Credential.Substring(userCreateCredentials.Credential.Length - 5)}";

                using (InventorySystemContext db = new InventorySystemContext())
                {
                    User user = new User
                    {
                        KeyPassword = "12345678",
                        UserName = partesCombinadas,
                        Name = userCreateCredentials.Name
                    };

                    db.Add(user);
                    db.SaveChanges();
                }

                return StatusCode(StatusCodes.Status201Created, new { message = "Usuario creado.", Usuario = partesCombinadas });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error de servidor: {ex}" });
            }
        }

        [HttpGet]
        [Route("/List_Users")]
        public dynamic ListUsers()
        {
            try
            {
                using(InventorySystemContext db = new InventorySystemContext())
                {
                    List<User> users = db.Users.ToList();
                    return StatusCode(StatusCodes.Status200OK, users );
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error de servidor: {ex}" });
            }
        }
    }

    public class UserCreateCredentials
    {
        public string Name { get; set; }
        public string Credential { get; set; }
    }

    public class UserCredentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class JwtSeccion
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }
    }
}
