using Microsoft.AspNetCore.Mvc;
using ArqPay.Models;
using ArqPay.Services;
using ArqPay.Models.Requests;
using ArqPay.Interfaces;
using ArqPay.Models.Responses;
using Microsoft.AspNetCore.Authorization;

namespace ArqPay.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class LoginController : ControllerBase
  {
    private readonly ILoginService _iLoginService;

    public LoginController(ILoginService iLoginService)
    {
      _iLoginService = iLoginService;
    }


    [HttpPost("/Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
      try
      {
        LoginResponse response = await _iLoginService.Login(loginRequest);
        return Ok(response);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("/Authenticate")]
    [AllowAnonymous]
    public async Task<IActionResult> Authenticate([FromBody] CreateRefreshTokenRequest createRefreshTokenRequest)
    {
      try
      {
        var response = await _iLoginService.Authenticate(createRefreshTokenRequest);
        return Ok(response);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }
  }
}
