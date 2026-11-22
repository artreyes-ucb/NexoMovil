namespace NexoMovil.Core.CustomEntities
{
    /// <summary>
    /// Parámetros del algoritmo PBKDF2 usados por PasswordService.
    /// </summary>
    public class PasswordOptions
    {
        public int SaltSize { get; set; }
        public int KeySize { get; set; }
        public int Iterations { get; set; }
    }
}
