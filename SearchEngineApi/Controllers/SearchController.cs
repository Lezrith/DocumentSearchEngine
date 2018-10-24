using DocumentSearchEngine;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SearchEngineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchEngine searchEngine;

        public SearchController(ISearchEngine searchEngine)
        {
            this.searchEngine = searchEngine;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<IEnumerable<string>> Get([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int perPage = 10)
        {
            if(page < 1)
            {
                return BadRequest("Page should be >= 1");
            }
            if(perPage < 1)
            {
                return BadRequest("PerPage should be >= 1");
            }
            try
            {
                var result = this.searchEngine.Search(query);
                var start = (page - 1) * perPage;
                return Ok(new { result.Query, Results = result.Results.Skip(start).Take(perPage) });
            }
            catch (ArgumentException)
            {
                return BadRequest("Your query is bad and you should feel bad");
            }
        }
    }
}