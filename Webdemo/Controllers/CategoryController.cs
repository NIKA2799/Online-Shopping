using Interface.Command;
using Interface.Model;
using Interface.Queries;
using Microsoft.AspNetCore.Mvc;
using Service.QueriesService;
using static System.Net.Mime.MediaTypeNames;


    namespace Webdemo.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class CategoryController : ControllerBase
        {
            private readonly ICategoryCommand _categoryCommand;
            private readonly ICategoryQurey _categoryQuery;

            public CategoryController(
                ICategoryCommand categoryCommand,
                ICategoryQurey categoryQuery)
            {
                _categoryCommand = categoryCommand;
                _categoryQuery = categoryQuery;
            }

            // GET: api/category
            [HttpGet]
            public IActionResult GetAll()
            {
                var categories = _categoryQuery.FindAll();
                return Ok(categories);
            }

            // GET: api/category/{id}
            [HttpGet("{id}")]
            public IActionResult Get(int id)
            {
                var category = _categoryQuery.Get(id);
                if (category == null)
                    return NotFound();

                return Ok(category);
            }

            // POST: api/category
            [HttpPost]
            public IActionResult Create([FromBody] CategoryModel model)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var id = _categoryCommand.Insert(model);
                return CreatedAtAction(nameof(Get), new { id }, model);
            }

            // PUT: api/category/{id}
            [HttpPut("{id}")]
            public IActionResult Update(int id, [FromBody] CategoryModel model)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _categoryCommand.Update(id, model);
                return NoContent();
            }

            // DELETE: api/category/{id}
            [HttpDelete("{id}")]
            public IActionResult Delete(int id)
            {
                _categoryCommand.Delete(id);
                return NoContent();
            }

            // GET: api/category/{categoryId}/products
            [HttpGet("{categoryId}/products")]
            public IActionResult GetProductsByCategory(int categoryId)
            {
                var products = _categoryQuery.GetProductsByCategory(categoryId);
                return Ok(products);
            }
        }
    }
