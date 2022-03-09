using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository
{
    public class PeliculaRepository : IRepositoryPelicula
    {
        private readonly ApplicationDBContext _db;

        public PeliculaRepository(ApplicationDBContext db)
        {
            _db = db;
        }
        public bool CreatePelicula(Pelicula pelicula)
        {
            _db.Pelicula.Add(pelicula);
            return Save();
        }

        public bool DeletePelicula(Pelicula pelicula)
        {
            _db.Pelicula.Remove(pelicula);
            return Save();
        }

        public bool ExistPelicula(string nombre)
        {
            return _db.Pelicula.Any(p => p.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
        }

        public bool ExistPelicula(int id)
        {
            return _db.Pelicula.Any(p => p.Id == id);
        }

        public Pelicula GetPelicula(int peliculaId)
        {
            return _db.Pelicula.FirstOrDefault(p => p.Id == peliculaId);
        }

        public ICollection<Pelicula> GetPeliculas()
        {
            return _db.Pelicula.OrderBy(p => p.Nombre).ToList();
        }

        public ICollection<Pelicula> GetPeliculasByCategoria(int categoriaId)
        {
            return _db.Pelicula.Include(c => c.Categoria).Where(p => p.CategoriaId == categoriaId).OrderBy(p => p.Nombre).ToList();
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public IEnumerable<Pelicula> GetPeliculaByName(string nombre)
        {
            IQueryable<Pelicula> query = _db.Pelicula;

            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(p => p.Nombre.Contains(nombre) || p.Descripcion.Contains(nombre));
            }

            return query.ToList();
        }

        public bool UpdatePelicula(Pelicula pelicula)
        {
            _db.Pelicula.Update(pelicula);
            return Save();
        }
    }
}
