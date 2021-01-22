using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using Neo4j.Driver.V1;
using Neo4jClient;
using Neo4jClient.Cypher;
using Soundable.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soundable.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly ILogger<AuthorController> _logger;
        private readonly IGraphClient _client; 


        public AuthorController(ILogger<AuthorController> logger, IGraphClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Author) RETURN n", test, Neo4jClient.Cypher.CypherResultMode.Set);
            List<Author> authors = ((IRawGraphClient)_client).ExecuteGetCypherResults<Author>(response).ToList();

            foreach(var a in authors)
            {
                Dictionary<string, object> test1 = new Dictionary<string, object>();
                test1.Add("name", a.name);
                var responsenew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Awards)< -[AUTHOR_AWARDS] - (author:Author { name: {name} }) RETURN n", test1, Neo4jClient.Cypher.CypherResultMode.Set);
                List<Awards> awards = ((IRawGraphClient)_client).ExecuteGetCypherResults<Awards>(responsenew).ToList();
                a.awards = awards;

                var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Label)< - [AUTHOR_LABEL] - (author:Author { name: {name} }) RETURN n", test1, Neo4jClient.Cypher.CypherResultMode.Set);
                Label label = ((IRawGraphClient)_client).ExecuteGetCypherResults<Label>(reponsenewnew).FirstOrDefault();
                a.label = label;

                var newresponse = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Album) -[ALBUM_AUTHOR] - >(author:Author { name: {name} }) RETURN n", test1, Neo4jClient.Cypher.CypherResultMode.Set);
                List<Album> albums = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(newresponse).ToList();
                a.albums = albums;
            }

            return Ok(authors);
        }

        [HttpPost("Create")]
        public async Task<ActionResult> CreateAuthorAsync([FromBody]Author author)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();
           
            var response = new Neo4jClient.Cypher.CypherQuery("CREATE (n:Author { name:'" + author.name
                                                            + "', surname:'" + author.surname
                                                            + "',nickname:'" + author.nickname + "',birthdate:'" + author.birthdate + "',ages:'" + author.ages + "',citizenship:'" + author.citizenship + "', networth:'" + author.networth + "', childrens:'" + author.childrens
                                                            + "', bornplace:'" + author.bornplace
                                                            + "'}) return n", test, Neo4jClient.Cypher.CypherResultMode.Set);
            List<Author> authors = ((IRawGraphClient)_client).ExecuteGetCypherResults<Author>(response).ToList();

            return Ok(authors);
        }
        
        [HttpDelete("Delete/{nickname}")]
        public async Task<ActionResult> DeleteAuthor(string nickname)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();
            test.Add("nickname", nickname);

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Author {nickname: {nickname}}) DETACH DELETE n", test, Neo4jClient.Cypher.CypherResultMode.Projection);

            List<Author> authors = ((IRawGraphClient)_client).ExecuteGetCypherResults<Author>(response).ToList();

            return Ok(authors);
        }

        [HttpPut("Update/{nickname}/{ages}")]
        public async Task<ActionResult> UpdateAuthor(string nickname, int ages)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();
           

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (author:Author { nickname: '" + nickname + "'}) SET author.ages = '" + ages + "' RETURN author", test, Neo4jClient.Cypher.CypherResultMode.Projection);

            List<Author> authors = ((IRawGraphClient)_client).ExecuteGetCypherResults<Author>(response).ToList();

            return Ok(authors);

        }

    }
}