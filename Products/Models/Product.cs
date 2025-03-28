using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace productos.Models
{
    public class Product
    {
        /// <summary>
        /// Identificador único del producto. Se genera automáticamente si no se proporciona.
        /// </summary>
        [Required]
        public Guid id { get; set; } //= Guid.NewGuid();

        /// <summary>
        /// Fecha de creación del producto. Se establece automáticamente cuando se crea el producto.
        /// </summary>
        [Required]
        public DateTimeOffset created_at { get; set; } //= DateTimeOffset.Now;

        /// <summary>
        /// Fecha de la última modificación del producto. Se actualiza cada vez que el producto es modificado.
        /// </summary>
        [Required]
        public DateTimeOffset modified_at { get; set; } //= DateTimeOffset.Now;

        /// <summary>
        /// Indica si el producto está eliminado. El valor predeterminado es falso.
        /// </summary>
        [Required]
        public bool is_deleted { get; set; } //= false;

        /// <summary>
        /// Fecha en la que el producto fue eliminado. Es nula si el producto no ha sido eliminado.
        /// </summary>
        public DateTimeOffset? deleted_at { get; set; }

        /// <summary>
        /// Tipo del producto (por ejemplo, "Electrónica", "Ropa"). Longitud máxima de 10 caracteres.
        /// </summary>
        [MaxLength(10)]
        [Required]
        public string type { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del producto. Longitud máxima de 255 caracteres.
        /// </summary>
        [MaxLength(255)]
        [Required]
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// Precio del producto, almacenado con dos decimales. El valor predeterminado es 0.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(8,2)")]  // Define la precisión y escala del precio en la base de datos.
        public decimal price { get; set; } = 0;

        /// <summary>
        /// Estado del producto, indicando si está activo o inactivo. El valor predeterminado es falso.
        /// </summary>
        [Required]
        public bool status { get; set; } = false;

        /// <summary>
        /// Descripción opcional del producto.
        /// </summary>
        public string? text { get; set; }

        /// <summary>
        /// Clave única del producto. Longitud máxima de 8 caracteres. Se genera automáticamente si no se proporciona.
        /// </summary>
        [MaxLength(8)]
        [Required]
        public string? Product_key { get; set; } = string.Empty;

        /// <summary>
        /// Enlace de imagen del producto. Longitud máxima de 200 caracteres.
        /// </summary>
        [MaxLength(200)]
        public string? image_lick { get; set; }
    }
}
