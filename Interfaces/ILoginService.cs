// Interfaces/IUserService.cs
using ArqPay.Models;
using ArqPay.Models.Requests;
using ArqPay.Models.Responses;

namespace ArqPay.Interfaces;

public interface ILoginService
{
  Task<LoginResponse> Login(LoginRequest request);
  
  Task<AuthenticateResponse> Authenticate(CreateRefreshTokenRequest request);
}
