using ApiPeliculas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository.IRepository
{
    public interface IRepositoryUsuario
    {
        ICollection<Usuario> GetUsuarios();
        Usuario GetUsuario(int usuarioId);
        bool ExistUsuario(string usuarioAccess);
        Usuario Registro(Usuario usuario, string password);
        Usuario Login(string usuario, string password);
        bool Save();
    }
}
