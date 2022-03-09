using ApiPeliculas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository.IRepository
{
    public interface IRepositoryPelicula
    {
        ICollection<Pelicula> GetPeliculas();
        ICollection<Pelicula> GetPeliculasByCategoria(int CategoriaId);
        Pelicula GetPelicula(int peliculaId);
        IEnumerable<Pelicula> GetPeliculaByName(string nombre);
        bool ExistPelicula(string nombre);
        bool ExistPelicula(int id);
        bool CreatePelicula(Pelicula pelicula);
        bool UpdatePelicula(Pelicula pelicula);
        bool DeletePelicula(Pelicula pelicula);
        bool Save();
    }
}
