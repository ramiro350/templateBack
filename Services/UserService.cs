using ArqPay.Models;
using ArqPay.Models.Requests;
using ArqPay.Helpers;
using ArqPay.Interfaces;

namespace ArqPay.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly PasswordHelper _passwordHelper;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
            _passwordHelper = new PasswordHelper();
        }

        public async Task<User?> GetUser(GetUserRequest request)
        {
            return await _repository.GetUserByEmail(request.Email);
        }

        public async Task<Guid> CreateUser(CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Cpf) || request.Cpf.Length != 11 || !request.Cpf.All(char.IsDigit))
            {
                throw new ArgumentException("CPF inválido. Deve conter exatamente 11 dígitos numéricos.");
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                try
                {
                    var mail = new System.Net.Mail.MailAddress(request.Email);
                    if (mail.Address != request.Email)
                        throw new ArgumentException("Formato de e-mail inválido.");
                }
                catch
                {
                    throw new ArgumentException("Formato de e-mail inválido.");
                }
            }

            if (request.DataNascimento >= DateTime.Now)
            {
                throw new ArgumentException("Data de nascimento não pode ser no futuro.");
            }


            string hashedPassword = _passwordHelper.HashPassword(request.Password, out string salt);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                passwordHash = hashedPassword,
                salt = salt,
                Cpf = request.Cpf,
                Nacionalidade = request.Nacionalidade,
                Naturalidade = request.Naturalidade,
                DataNascimento = request.DataNascimento,
                Sexo = request.Sexo,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            return await _repository.CreateUser(user);
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _repository.GetAllUsers();
        }

        public async Task<User?> GetUserById(Guid id)
        {
            User user = await _repository.GetUserById(id);

            if (user is null)
            {
                throw new Exception("Usuário não encontrado");    
            }

            return user;
        }

        public async void UpdateUser(UpdateUserRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Cpf) && (request.Cpf.Length != 11 || !request.Cpf.All(char.IsDigit)))
            {
                throw new ArgumentException("CPF inválido. Deve conter exatamente 11 dígitos numéricos.");
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                try
                {
                    var mail = new System.Net.Mail.MailAddress(request.Email);
                    if (mail.Address != request.Email)
                        throw new ArgumentException("Formato de e-mail inválido.");
                }
                catch
                {
                    throw new ArgumentException("Formato de e-mail inválido.");
                }
            }

            if (request.DataNascimento.HasValue && request.DataNascimento >= DateTime.Now)
            {
                throw new ArgumentException("Data de nascimento não pode ser no futuro.");
            }

            User getUser = await _repository.GetUserById(request.Id);
            if (getUser is null)
            {
                throw new Exception("Usuário não existente");
            }

            User userWithCredential = await _repository.GetUserWithCredentials(getUser.Email);

            string newPasswordHash = userWithCredential.passwordHash;
            string newSalt = userWithCredential.salt;

            // Only hash and update password/salt if a NON-EMPTY password string is provided
            if (!string.IsNullOrEmpty(request.Password))
            {
                // Hash the new password and get the salt
                newPasswordHash = _passwordHelper.HashPassword(request.Password, out string salt);
                newSalt = salt;
            }


            var user = new User
            {
                Id = request.Id,
                Name = request.Name ?? getUser.Name,
                Email = request.Email ?? getUser.Email,
                passwordHash = newPasswordHash,
                salt = newSalt,
                Cpf = request.Cpf ?? getUser.Cpf,
                Nacionalidade = request.Nacionalidade ?? getUser.Nacionalidade,
                Naturalidade = request.Naturalidade ?? getUser.Naturalidade,
                DataNascimento = (DateTime)(request.DataNascimento.HasValue ? request.DataNascimento : getUser.DataNascimento),
                Sexo = request.Sexo ?? getUser.Sexo,
                UpdateAt = DateTime.Now
            };

            await _repository.UpdateUser(user);
        }

        public async Task<int> DeleteUser(Guid id)
        {
            User user = await _repository.GetUserById(id);

            if (user is null)
            {
                throw new Exception("Usuário não encontrado");    
            }

            return await _repository.DeleteUser(id);
        }

        
        public async Task<User?> GetUserWithCredentials(string email)
        {
            User user = await _repository.GetUserWithCredentials(email);

            if (user is null)
            {
                throw new Exception("Usuário não encontrado");    
            }

            return user;
        }
    }
}
