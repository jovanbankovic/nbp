using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neo4jClient;
using Soundable.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soundable.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenreController : Controller
    {
        private readonly ILogger<GenreController> _logger;
        private readonly IGraphClient _client;


        public GenreController(ILogger<GenreController> logger, IGraphClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Genre) RETURN n", test, Neo4jClient.Cypher.CypherResultMode.Set);
            List<Genre> genres = ((IRawGraphClient)_client).ExecuteGetCypherResults<Genre>(response).ToList();

            foreach (var a in genres)
            {
                Dictionary<string, object> test1 = new Dictionary<string, object>();
                test1.Add("name", a.name);
                var responsenew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Album) - [ALBUM_GENRE] - >(genre:Genre { name: {name} }) RETURN n", test1, Neo4jClient.Cypher.CypherResultMode.Set);
                List<Album> albums = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(responsenew).ToList();
                a.albums = albums;
            }

            return Ok(genres);
        }

        [HttpPost("Create")]
        public async Task<ActionResult> CreateGenreAsync([FromBody] Genre genre)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var response = new Neo4jClient.Cypher.CypherQuery("CREATE (n:Genre { name:'" + genre.name
                                                            + "' }) return n", test, Neo4jClient.Cypher.CypherResultMode.Set);
            List<Genre> genres = ((IRawGraphClient)_client).ExecuteGetCypherResults<Genre>(response).ToList();

            /*var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Album { name: '" + album.name + "' }),(g:Author { nickname: '" + nickname + "' }) MERGE (a)-[r:ALBUM_AUTHOR]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Album responsealbum = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew).FirstOrDefault();

            var reponsenewnew1 = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Album { name: '" + album.name + "' }),(g:Genre { name: '" + genreName + "' }) MERGE (a)-[r:ALBUM_GENRE]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Album responsealbum1 = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew1).FirstOrDefault();*/


            return Ok(genres);
        }
        [HttpDelete("Delete/{GenreName}")]
        public async Task<ActionResult> DeleteGenre(string GenreName)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();
            test.Add("name", GenreName);

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Genre {name: {name}}) DETACH DELETE n", test, Neo4jClient.Cypher.CypherResultMode.Projection);

            List<Genre> genres = ((IRawGraphClient)_client).ExecuteGetCypherResults<Genre>(response).ToList();

            return Ok(genres);
        }

        [HttpPut("Update/{GenreName}/{NewGenreName}")]
        public async Task<ActionResult> UpdateGenre(string GenreName, string NewGenreName)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();


            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (genre:Genre { name: '" + GenreName + "'}) SET genre.name = '" + NewGenreName + "' RETURN genre", test, Neo4jClient.Cypher.CypherResultMode.Projection);

            List<Genre> genres = ((IRawGraphClient)_client).ExecuteGetCypherResults<Genre>(response).ToList();

            return Ok(genres);

        }
    }

}

