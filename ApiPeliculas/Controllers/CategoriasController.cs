using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [Authorize]
    [Route("api/categorias")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiPeliculasCategorias")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class CategoriasController : Controller
    {
        private readonly IRepositoryCategoria _catRepo;
        private readonly IMapper _mapper;

        public CategoriasController(IRepositoryCategoria catRep, IMapper mapper)
        {
            _catRepo = catRep;
            _mapper = mapper;
        }

        #region GET
        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<CategoriaDTO>))]
        [ProducesResponseType(400)]
        public IActionResult GetCategorias()
        {
            var listaCategorias = _catRepo.GetCategorias();

            var listaCategoriasDTO = new List<CategoriaDTO>();

            foreach (var lista in listaCategorias)
            {
                listaCategoriasDTO.Add(_mapper.Map<CategoriaDTO>(lista));
            }

            return Ok(listaCategoriasDTO);
        }

        #endregion

        #region CRUD
        /// <summary>
        /// Get Categoria by ID
        /// </summary>
        /// <param name="categoriaId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{categoriaId:int}", Name ="GetCategoria")]
        [ProducesResponseType(200, Type = typeof(List<CategoriaDTO>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetCategoria(int categoriaId)
        {
            var categoria = _catRepo.GetCategoria(categoriaId);

            if (categoria == null) return NotFound();

            return Ok(_mapper.Map<CategoriaDTO>(categoria));
        }

        /// <summary>
        /// Create a new Categoria
        /// </summary>
        /// <param name="categoriaDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(List<CategoriaDTO>))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public IActionResult CreateCategoria([FromBody] CategoriaDTO categoriaDTO)
        {
            if (categoriaDTO == null) return BadRequest(ModelState);

            if (_catRepo.ExistCategory(categoriaDTO.Nombre))
            {
                ModelState.AddModelError("", "La Categoría ya existe");
                return StatusCode(404, ModelState);
            }

            var categoria = _mapper.Map<Categoria>(categoriaDTO);
            categoria.FechaCreacion = DateTime.Now;

            if (!_catRepo.CreateCategory(categoria))
            {
                ModelState.AddModelError("", $"Algo salió mal guardando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategoria", new { categoriaId = categoria.Id }, categoria);
        }

        /// <summary>
        /// Update a Categoria by ID
        /// </summary>
        /// <param name="categoriaId"></param>
        /// <param name="categoriaDTO"></param>
        /// <returns></returns>
        [HttpPatch("{categoriaId:int}", Name = "UpdateCategoria")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCategoria(int categoriaId, [FromBody] CategoriaDTO categoriaDTO)
        {
            if (categoriaDTO == null || categoriaId != categoriaDTO.Id) return BadRequest(ModelState);

            var categoriaOld = _catRepo.GetCategoria(categoriaId);

            categoriaDTO.FechaCreacion = categoriaOld.FechaCreacion;

            var categoria = _mapper.Map<Categoria>(categoriaDTO);

            if (!_catRepo.UpdateCategory(categoria))
            {
                ModelState.AddModelError("", $"Algo salió mal actualizando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a Categoria by ID
        /// </summary>
        /// <param name="categoriaId"></param>
        /// <returns></returns>
        [HttpDelete("{categoriaId:int}", Name = "DeleteCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCategoria(int categoriaId)
        {
            if (!_catRepo.ExistCategory(categoriaId))return NotFound();
            
            var categoria = _catRepo.GetCategoria(categoriaId);

            if (!_catRepo.DeleteCategory(categoria))
            {
                ModelState.AddModelError("", $"Algo salió mal eliminando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        #endregion
    }
}