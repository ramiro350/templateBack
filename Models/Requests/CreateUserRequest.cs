namespace ArqPay.Models.Requests

{
    public class CreateUserRequest {
        public string Name { get; set;}
        public string? Email { get; set;}
        public string Password { get; set;}
        public string Cpf { get; set;}
        public string? Nacionalidade { get; set;}
        public string? Naturalidade { get; set;}
        public string? Sexo { get; set; }
        public DateTime DataNascimento { get; set; }
    }
}
