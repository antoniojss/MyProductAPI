using Microsoft.AspNetCore.Mvc;
using MyProductAPI.Application.UseCases.Login.WithEmailAndPassword;
using MyProductAPI.Communication.Requests;
using MyProductAPI.Communication.Responses;
using MyProductAPI.Exception.ExceptionBase;

namespace MyProductAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        [HttpPost("login")]
        [ProducesResponseType(typeof(ResponseRegistredUserJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> Login(
            [FromServices] ILoginWithEmailAndPasswordUseCase useCase,
            [FromBody] RequestLoginJson request)
        {
            var response = await useCase.Execute(request);

            return Ok(response);
        }

    }
}
