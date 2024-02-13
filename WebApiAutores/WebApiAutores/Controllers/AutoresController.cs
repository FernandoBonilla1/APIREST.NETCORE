using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers
{

    [ApiController]
    [Route("[controller]")] //autores => ruta
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IServicio servicio;
        private readonly ServicioTransient servicioTransient;
        private readonly ServicioScoped servicioScoped;
        private readonly ServicioSingleton servicioSingleton;
        private readonly ILogger<AutoresController> logger;

        //Instancio DBCONTEXT
        public AutoresController(ApplicationDbContext context, IServicio servicio,
            ServicioTransient servicioTransient, ServicioScoped servicioScoped,
            ServicioSingleton servicioSingleton, ILogger<AutoresController> logger)
        {
            this.context = context;
            this.servicio = servicio;
            this.servicioTransient = servicioTransient;
            this.servicioScoped = servicioScoped;
            this.servicioSingleton = servicioSingleton;
            this.logger = logger;
        }

        [HttpGet("GUID")]
        public ActionResult ObtenerGuids()
        {
            return Ok( new {
                AutoresControllerTransient = servicioTransient.Guid,
                ServicioA_Transient = servicio.ObtenerTransient(),
                AutoresControllerScoped = servicioScoped.Guid,
                ServicioA_Scoped = servicio.ObtenerScoped(),
                AutoresControllerSingleton = servicioSingleton.Guid,
                ServicioA_Singleton = servicio.ObtenerSingleton()
            });
        }

        [HttpGet]
        public async Task<ActionResult<List<Autor>>> Get()
        {
            logger.LogInformation("Estamos obteniendo los autores");
            return await context.Autores.ToListAsync();
        }

        [HttpGet("PrimerAutor")]
        public async Task<ActionResult<List<Autor>>> PrimerAutor()
        {
            return await context.Autores.Include(x => x.Libros).ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Autor>> Get (int id)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            if(autor == null)
            {
                return NotFound();
            }

            return autor;
        }

        [HttpGet("{nombre}")]
        public async Task<ActionResult<Autor>> Get(string nombre)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));
            if (autor == null)
            {
                return NotFound();
            }

            return autor;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Autor autor)
        {

            var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);

            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {autor.Nombre}");
            }
            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")] //api/autores/:id
        public async Task<ActionResult> Put(Autor autor, int id)
        {
            if (autor.Id != id)
            {
                return BadRequest("El id del autor no coincide con el id de la URL");
            }

            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")] //api/autores/:id
        public async Task<ActionResult> Delete (int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }

    }
}
