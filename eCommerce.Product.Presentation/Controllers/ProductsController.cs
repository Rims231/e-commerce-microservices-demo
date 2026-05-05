using eCommerce.Product.Domain.Interfaces;
using eCommerce.SharedLibrary.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.Product.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Domain.Entities.Product>>> GetAll()
        {
            var products = await productInterface.GetAllAsync();
            return products.Any()
                ? Ok(products)
                : NotFound("No products found");
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Domain.Entities.Product>> GetById(int id)
        {
            var product = await productInterface.GetByIdAsync(id);
            return product is not null
                ? Ok(product)
                : NotFound("Product not found");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> Create([FromBody] Domain.Entities.Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await productInterface.CreateAsync(product);
            return response.flag
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> Update([FromBody] Domain.Entities.Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await productInterface.UpdateAsync(product);
            return response.flag
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> Delete(int id)
        {
            var product = await productInterface.GetByIdAsync(id);
            if (product is null)
                return NotFound("Product not found");

            var response = await productInterface.DeleteAsync(product);
            return response.flag
                ? Ok(response)
                : BadRequest(response);
        }
    }
}