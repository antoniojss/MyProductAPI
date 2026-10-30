using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProductAPI.Application.UseCases.Product.DeleteById;
using MyProductAPI.Application.UseCases.Product.GetById;
using MyProductAPI.Application.UseCases.Product.Register;
using MyProductAPI.Application.UseCases.Product.UpdateById;
using MyProductAPI.Communication.Requests;
using MyProductAPI.Communication.Responses;
using MyProductAPI.Domain.Repositories.Product;
using MyProductAPI.Exception.ExceptionBase;

namespace MyProductAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ProdutosController : ControllerBase
    {

        private readonly IProductWriteOnlyRepository _produtoService;

        public ProdutosController(IProductWriteOnlyRepository produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseRegistredProductJson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(
                                    [FromBody] RequestProductJson request,
                                    [FromServices] IRegisterProductUseCase useCase)
        {
            var result = await useCase.Execute(request);
            // registra o produto  
            return Created(result.IdBarra, result);
        }


        [HttpGet("{idBarra}")]
        [ProducesResponseType(typeof(ResponseProductJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(
                                [FromRoute] string idBarra,
                                [FromServices] IGetProductByIdUseCase useCase)
        {
            var product = await useCase.Execute(idBarra);
            return Ok(product);
        }

        [HttpPut("{idBarra}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(
                       [FromRoute] string idBarra,
                       [FromBody] RequestProductJson request,
                       [FromServices] IUpdateProductByIdUseCase  useCase)
        {
            await useCase.Execute(idBarra, request);
            return NoContent();
        }

        [HttpDelete("{idBarra}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
                      [FromRoute] string idBarra,
                      [FromServices] IDeleteProductByIdUseCase useCase)
        {
            await useCase.Execute(idBarra);
            return NoContent();
        }
    }
}
