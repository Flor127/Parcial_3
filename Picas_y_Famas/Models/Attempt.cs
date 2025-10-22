namespace Picas_y_Famas.Models
{
    public class Attempt
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public string IntentoNumero{ get; set; }
        public DateTime AttemptedAt { get; set; }
    }
}
