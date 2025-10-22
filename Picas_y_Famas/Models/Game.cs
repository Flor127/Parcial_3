namespace Picas_y_Famas.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string NumeroSecreto { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Fin { get; set; }
    }
}
