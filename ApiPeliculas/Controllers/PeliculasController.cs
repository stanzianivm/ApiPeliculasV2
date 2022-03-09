using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [Authorize]
    [Route("api/peliculas")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiPeliculas")]
    public class PeliculasController : ControllerBase
    {
        private readonly IRepositoryPelicula _peliRepo;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostingEnviroment;

        public PeliculasController(IRepositoryPelicula peliRep, IMapper mapper, IWebHostEnvironment hostingEnviroment)
        {
            _peliRepo = peliRep;
            _mapper = mapper;
            _hostingEnviroment = hostingEnviroment;
        }

        #region CRUD

        [HttpPost]
        public IActionResult CreatePelicula([FromForm] PeliculaCreateDTO peliculaCreateDTO)
        {
            if (peliculaCreateDTO == null) return BadRequest(ModelState);

            if (_peliRepo.ExistPelicula(peliculaCreateDTO.Nombre))
            {
                ModelState.AddModelError("", "La Película ya existe");
                return StatusCode(404, ModelState);
            }

            /* Subida de archivos */

            var file = peliculaCreateDTO.Foto;
            string pathMean = _hostingEnviroment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            if (file.Length > 0)
            {
                // Nueva imagen.
                // NewGuid para un nombre único.
                var namePhoto = Guid.NewGuid().ToString();
                var uploads = Path.Combine(pathMean, @"Photos");
                var extension = Path.GetExtension(files[0].FileName);

                using (var fileStreams = new FileStream(Path.Combine(uploads, namePhoto + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStreams);
                }

                peliculaCreateDTO.RutaImagen = @"\Photos\" + namePhoto + extension;
            }

            var pelicula = _mapper.Map<Pelicula>(peliculaCreateDTO);
            pelicula.FechaCreacion = DateTime.Now;

            if (!_peliRepo.CreatePelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salió mal guardando el registro {pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetPelicula", new { peliculaId = pelicula.Id }, pelicula);
        }

        [HttpPatch("{peliculaId:int}", Name = "UpdatePelicula")]
        public IActionResult UpdatePelicula(int peliculaId, [FromBody] PeliculaUpdateDTO PeliculaUpdateDTO)
        {
            if (PeliculaUpdateDTO == null || peliculaId != PeliculaUpdateDTO.Id) return BadRequest(ModelState);

            var pelicula = _mapper.Map<Pelicula>(PeliculaUpdateDTO);

            if (!_peliRepo.UpdatePelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salió mal actualizando el registro {pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{peliculaId:int}", Name = "DeletePelicula")]
        public IActionResult DeletePelicula(int peliculaId)
        {
            if (!_peliRepo.ExistPelicula(peliculaId)) return NotFound();

            var pelicula = _peliRepo.GetPelicula(peliculaId);

            if (!_peliRepo.DeletePelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salió mal eliminando el registro {pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        #endregion

        #region Get
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetPeliculas()
        {
            var listaPeliculas = _peliRepo.GetPeliculas();

            var listaPeliculasDTO = new List<PeliculaDTO>();

            foreach (var lista in listaPeliculas)
            {
                listaPeliculasDTO.Add(_mapper.Map<PeliculaDTO>(lista));
            }

            return Ok(listaPeliculasDTO);
        }

        [AllowAnonymous]
        [HttpGet("{peliculaId:int}", Name = "GetPelicula")]
        public IActionResult GetPelicula(int peliculaId)
        {
            var pelicula = _peliRepo.GetPelicula(peliculaId);

            if (pelicula == null) return NotFound();

            return Ok(_mapper.Map<PeliculaDTO>(pelicula));
        }

        [AllowAnonymous]
        [HttpGet("GetPeliculasByCategoria/{categoriaId:int}")]
        public IActionResult GetPeliculasByCategoria(int categoriaId)
        {
            var listPeliculas = _peliRepo.GetPeliculasByCategoria(categoriaId);

            if (listPeliculas == null) return NotFound();

            var itemPelicula = new List<PeliculaDTO>();

            foreach (var item in listPeliculas)
            {
                itemPelicula.Add(_mapper.Map<PeliculaDTO>(item));
            }

            return Ok(itemPelicula);
        }

        [AllowAnonymous]
        [HttpGet("GetPeliculaByName")]
        public IActionResult GetPeliculaByName(string name)
        {
            try
            {
                var request = _peliRepo.GetPeliculaByName(name);

                if (request.Any()) return Ok(request);

                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando los datos de la aplicación");
            }
        }

        #endregion
    }
}