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
    public class AwardController : Controller
    {
        private readonly ILogger<AwardController> _logger;
        private readonly IGraphClient _client;

        public AwardController(ILogger<AwardController> logger, IGraphClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Awards) RETURN n", test, Neo4jClient.Cypher.CypherResultMode.Set);
            List<Awards> awards = ((IRawGraphClient)_client).ExecuteGetCypherResults<Awards>(response).ToList();

            foreach (var a in awards)
            {
                Dictionary<string, object> test1 = new Dictionary<string, object>();
                test1.Add("name", a.name);
                var responsenew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Song) - [SONG_AWARD] - >(awards:Awards { name: {name} }) RETURN n", test1, Neo4jClient.Cypher.CypherResultMode.Set);
                List<Song> songs = ((IRawGraphClient)_client).ExecuteGetCypherResults<Song>(responsenew).ToList();
                a.songs = songs;

                var responsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Author) - [AUTHOR_AWARDS] - >(awards:Awards { name: {name} }) RETURN n", test1, Neo4jClient.Cypher.CypherResultMode.Set);
                List<Author> authors = ((IRawGraphClient)_client).ExecuteGetCypherResults<Author>(responsenew).ToList();
                a.authors = authors;
            }

            return Ok(awards);
        }
        [HttpPost("Create")]
        public async Task<ActionResult> CreateAwardAsync([FromBody] Awards award)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var response = new Neo4jClient.Cypher.CypherQuery("CREATE (n:Awards { name:'" + award.name
                                                            + "', date:'" + award.date
                                                            + "',city:'" + award.city + "'}) return n", test, Neo4jClient.Cypher.CypherResultMode.Set);
            List<Awards> awards = ((IRawGraphClient)_client).ExecuteGetCypherResults<Awards>(response).ToList();

            /*var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Album { name: '" + album.name + "' }),(g:Author { nickname: '" + nickname + "' }) MERGE (a)-[r:ALBUM_AUTHOR]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Album responsealbum = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew).FirstOrDefault();

            var reponsenewnew1 = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Album { name: '" + album.name + "' }),(g:Genre { name: '" + genreName + "' }) MERGE (a)-[r:ALBUM_GENRE]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Album responsealbum1 = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew1).FirstOrDefault();*/


            return Ok(awards);
        }

        [HttpPost("Connect/{AwardName}/{AuthorNickname}/{SongName}")]
        public async Task<ActionResult> ConnectAwardsAsync(string AwardName, string AuthorNickname, string SongName)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Song { name: '" + SongName + "' }),(g:Awards { name: '" + AwardName + "' }) MERGE (a)-[r:SONG_AWARD]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Song responsesong = ((IRawGraphClient)_client).ExecuteGetCypherResults<Song>(reponsenewnew).FirstOrDefault();

            var reponsenewnew1 = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Author { nickname: '" + AuthorNickname + "' }),(g:Awards { name: '" + AwardName + "' }) MERGE (a)-[r:AUTHOR_AWARDS]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Author responsealbum1 = ((IRawGraphClient)_client).ExecuteGetCypherResults<Author>(reponsenewnew1).FirstOrDefault();


            return Ok(responsesong);
        }


        [HttpDelete("Delete/{AwardName}")]
        public async Task<ActionResult> DeleteAward(string AwardName)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();
            test.Add("name", AwardName);

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Awards {name: {name}}) DETACH DELETE n", test, Neo4jClient.Cypher.CypherResultMode.Projection);

            List<Awards> awards = ((IRawGraphClient)_client).ExecuteGetCypherResults<Awards>(response).ToList();

            return Ok(awards);
        }

        [HttpPut("Update/{AwardName}/{NewCity}")]
        public async Task<ActionResult> UpdateAward(string AwardName, string NewCity)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();


            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (award:Awards { name: '" + AwardName + "'}) SET award.city = '" + NewCity + "' RETURN award", test, Neo4jClient.Cypher.CypherResultMode.Projection);

            List<Awards> awards = ((IRawGraphClient)_client).ExecuteGetCypherResults<Awards>(response).ToList();

            return Ok(awards);
        }
    }
}

