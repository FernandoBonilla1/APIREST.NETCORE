using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Autor: IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo nombre es requerido.")]
        [StringLength(maximumLength:5, ErrorMessage = "El campo nombre no debe tener mas de 5 caracteres.")]
        public string Nombre { get; set; }
        //[Range(18,120)]
        //[NotMapped] //No afecta como una columna en la base de datos
        //public int? Edad {  get; set; }
        //[CreditCard]
        //[NotMapped]
        //public string? TarjetaDeCredito { get; set; }
        //[Url]
        //[NotMapped]
        //public string? URL { get; set; }
        public List<Libro>? Libros { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraLetra = Nombre[0].ToString();

                if(primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra debe ser mayúscula",
                        new string[] { nameof(Nombre) });
                }
            }
        }
    }
}
