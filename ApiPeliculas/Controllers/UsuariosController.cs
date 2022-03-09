using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ApiPeliculas.Controllers
{
    [Authorize]
    [Route("api/usuarios")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiPeliculasUsuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly IRepositoryUsuario _userRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UsuariosController(IRepositoryUsuario peliRep, IMapper mapper, IConfiguration config)
        {
            _userRepo = peliRep;
            _mapper = mapper;
            _config = config;
        }

        #region CRUD
        [AllowAnonymous]
        [HttpPost("Registro")]
        public IActionResult Registro(UsuarioAuthDTO UsuarioAuthDTO)
        {
            if (_userRepo.ExistUsuario(UsuarioAuthDTO.User.ToLower()))
            {
                return BadRequest("El usuario ya existe");
            }

            var usuarioToCreate = new Usuario()
            {
                UserAccess = UsuarioAuthDTO.User
            };

            var usuarioNew = _userRepo.Registro(usuarioToCreate, UsuarioAuthDTO.Password);
            return Ok(usuarioNew);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UsuarioAuthLoginDTO UsuarioAuthLoginDTO)
        {
            var userFromRepo = _userRepo.Login(UsuarioAuthLoginDTO.User, UsuarioAuthLoginDTO.Password);

            if (userFromRepo == null) return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.UserAccess.ToString())
            };

            // Generación de token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescription);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });            
        }

        #endregion

        #region GET
        [HttpGet]
        public IActionResult GetUsuarios()
        {
            var listaUsuarios = _userRepo.GetUsuarios();

            var listaUsuariosDTO = new List<UsuarioDTO>();

            foreach (var lista in listaUsuarios)
            {
                listaUsuariosDTO.Add(_mapper.Map<UsuarioDTO>(lista));
            }

            return Ok(listaUsuariosDTO);
        }

        [HttpGet("{usuarioId:int}", Name = "GetUsuario")]
        public IActionResult GetUsuario(int usuarioId)
        {
            var usuario = _userRepo.GetUsuario(usuarioId);

            if (usuario == null) return NotFound();

            return Ok(_mapper.Map<UsuarioDTO>(usuario));
        }

        #endregion
    }
}