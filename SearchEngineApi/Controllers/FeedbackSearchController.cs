using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentSearchEngine;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SearchEngineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackSearchController : ControllerBase
    {
        private readonly ISearchEngine searchEngine;

        public FeedbackSearchController(ISearchEngine searchEngine)
        {
            this.searchEngine = searchEngine;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult<IEnumerable<string>> Get(
            [FromQuery] string query,
            [FromQuery] string positive,
            [FromQuery] string negative,
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10)
        {
            if (page < 1)
            {
                return BadRequest("Page should be >= 1");
            }
            if (perPage < 1)
            {
                return BadRequest("PerPage should be >= 1");
            }
            try
            {
                var positiveSplit = (positive ?? "").Split(',');
                var negativeSplit = (negative ?? "").Split(',');
                var start = (page - 1) * perPage;
                var result = this.searchEngine.SearchWithFeedback(query, positiveSplit, negativeSplit);
                return Ok(new { result.Query, Results = result.Results.Skip(start).Take(perPage) });
            }
            catch (ArgumentException)
            {
                return BadRequest("Your query is bad and you should feel bad");
            }
        }
    }
}