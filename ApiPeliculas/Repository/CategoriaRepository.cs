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
    public class CategoriaRepository : IRepositoryCategoria
    {
        private readonly ApplicationDBContext _db;

        public CategoriaRepository(ApplicationDBContext db)
        {
            _db = db;
        }

        public bool CreateCategory(Categoria category)
        {
            _db.Categoria.Add(category);
            return Save();
        }

        public bool DeleteCategory(Categoria category)
        {
            _db.Categoria.Remove(category);
            return Save();
        }

        public bool ExistCategory(string nombre)
        {
            return _db.Categoria.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
        }

        public bool ExistCategory(int id)
        {
            return _db.Categoria.Any(c => c.Id == id);
        }

        public Categoria GetCategoria(int categoriaId)
        {
            return _db.Categoria.AsNoTracking().FirstOrDefault(c => c.Id == categoriaId);
        }

        public ICollection<Categoria> GetCategorias()
        {
            return _db.Categoria.OrderBy(c => c.Nombre).ToList();
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateCategory(Categoria category)
        {
            _db.Categoria.Update(category);
            return Save();
        }
    }
}
