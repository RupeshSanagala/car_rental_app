using Car_Rental_Backend_Application.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Car_Rental_Backend_Application.Data.Entities
{
    [Table("Cars")]
    public class Car
    {
        internal string License_Plate;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Car_ID { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required] public string Model { get; set;}

        [Required]
        public decimal PricePerDay { get; set; }

        [Required]
        public string Avaliability_Status {  get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
