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
    public class LabelController : Controller
    {
        private readonly ILogger<LabelController> _logger;
        private readonly IGraphClient _client;


        public LabelController(ILogger<LabelController> logger, IGraphClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Label) RETURN n", test, Neo4jClient.Cypher.CypherResultMode.Set);
            List<Label> labels = ((IRawGraphClient)_client).ExecuteGetCypherResults<Label>(response).ToList();

            foreach (var a in labels)
            {
                Dictionary<string, object> test1 = new Dictionary<string, object>();
                test1.Add("name", a.name);
                var responsenew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Author) - [AUTHOR_LABEL] - >(label:Label { name: {name} }) RETURN n", test1, Neo4jClient.Cypher.CypherResultMode.Set);
                List<Author> authors = ((IRawGraphClient)_client).ExecuteGetCypherResults<Author>(responsenew).ToList();
                a.authors = authors;
            }

            return Ok(labels);
        }

        [HttpPost("Create")]
        public async Task<ActionResult> CreateLabelAsync([FromBody] Label label)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var response = new Neo4jClient.Cypher.CypherQuery("CREATE (n:Label { name:'" + label.name
                                                            + "', founded:'" + label.founded
                                                            + "',founder:'" + label.founder + "',country:'" + label.country + "'}) return n", test, Neo4jClient.Cypher.CypherResultMode.Set);
            List<Label> labels = ((IRawGraphClient)_client).ExecuteGetCypherResults<Label>(response).ToList();

            /*var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Album { name: '" + album.name + "' }),(g:Author { nickname: '" + nickname + "' }) MERGE (a)-[r:ALBUM_AUTHOR]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Album responsealbum = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew).FirstOrDefault();

            var reponsenewnew1 = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Album { name: '" + album.name + "' }),(g:Genre { name: '" + genreName + "' }) MERGE (a)-[r:ALBUM_GENRE]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Album responsealbum1 = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew1).FirstOrDefault();*/


            return Ok(labels);
        }

        [HttpPost("Connect/{LabelName}/{AuthorNickname}")]
        public async Task<ActionResult> ConnectLabelAsync(string LabelName, string AuthorNickname)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Author { nickname: '" + AuthorNickname + "' }),(g:Label { name: '" + LabelName + "' }) MERGE (a)-[r:AUTHOR_LABEL]->(g) return g", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Label responseLabel = ((IRawGraphClient)_client).ExecuteGetCypherResults<Label>(reponsenewnew).FirstOrDefault();

            /*var reponsenewnew1 = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Author { nickname: '" + AuthorNickname + "' }),(g:Awards { name: '" + AwardName + "' }) MERGE (a)-[r:AUTHOR_AWARDS]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Author responsealbum1 = ((IRawGraphClient)_client).ExecuteGetCypherResults<Author>(reponsenewnew1).FirstOrDefault();*/


            return Ok(responseLabel);
        }
        [HttpDelete("Delete/{LabelName}")]
        public async Task<ActionResult> DeleteLabel(string LabelName)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();
            test.Add("name", LabelName);

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Label {name: {name}}) DETACH DELETE n", test, Neo4jClient.Cypher.CypherResultMode.Projection);

            List<Label> labels = ((IRawGraphClient)_client).ExecuteGetCypherResults<Label>(response).ToList();

            return Ok(labels);
        }

        [HttpPut("Update/{LabelName}/{NewLabelName}")]
        public async Task<ActionResult> UpdateLabel(string LabelName, string NewLabelName)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();


            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (label:Label { name: '" + LabelName + "'}) SET label.name = '" + NewLabelName + "' RETURN label", test, Neo4jClient.Cypher.CypherResultMode.Projection);

            List<Label> labels = ((IRawGraphClient)_client).ExecuteGetCypherResults<Label>(response).ToList();

            return Ok(labels);

        }
    }
}
