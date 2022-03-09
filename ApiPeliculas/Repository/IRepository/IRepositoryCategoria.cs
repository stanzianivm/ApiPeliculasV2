using ApiPeliculas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository.IRepository
{
    public interface IRepositoryCategoria
    {
        ICollection<Categoria> GetCategorias();
        Categoria GetCategoria(int categoriaId);
        bool ExistCategory(string nombre);
        bool ExistCategory(int id);
        bool CreateCategory(Categoria category);
        bool UpdateCategory(Categoria category);
        bool DeleteCategory(Categoria category);
        bool Save();
    }
}
