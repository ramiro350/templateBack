namespace ArqPay.Models

{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string passwordHash { get; set; } = string.Empty;
        public string salt { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Nacionalidade { get; set; } = string.Empty;
        public string Naturalidade { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
