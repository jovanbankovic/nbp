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
    public class AlbumController : Controller
    {
        private readonly ILogger<AlbumController> _logger;
        private readonly IGraphClient _client;


        public AlbumController(ILogger<AlbumController> logger, IGraphClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Album) RETURN n", test, Neo4jClient.Cypher.CypherResultMode.Set);
            List<Album> albums = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(response).ToList();

            foreach (var a in albums)
            {
                Dictionary<string, object> test1 = new Dictionary<string, object>();
                test1.Add("name", a.name);
                var responsenew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Song)< -[ALBUM_SONG] - (album:Album { name: {name} }) RETURN n", test1, Neo4jClient.Cypher.CypherResultMode.Set);
                List<Song> songs = ((IRawGraphClient)_client).ExecuteGetCypherResults<Song>(responsenew).ToList();
                a.songs = songs;

                var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Author)< - [ALBUM_AUTHOR] - (album:Album { name: {name} }) RETURN n", test1, Neo4jClient.Cypher.CypherResultMode.Set);
                Author author = ((IRawGraphClient)_client).ExecuteGetCypherResults<Author>(reponsenewnew).FirstOrDefault();
                a.author = author;

                var reponsenewnew1 = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Genre)< - [ALBUM_GENRE] - (album:Album { name: {name} }) RETURN n", test1, Neo4jClient.Cypher.CypherResultMode.Set);
                Genre genre = ((IRawGraphClient)_client).ExecuteGetCypherResults<Genre>(reponsenewnew1).FirstOrDefault();
                a.genre = genre;
            }

            return Ok(albums);
        }

        [HttpPost("Create")]
        public async Task<ActionResult> CreateAlbumAsync([FromBody] Album album)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var response = new Neo4jClient.Cypher.CypherQuery("CREATE (n:Album { name:'" + album.name
                                                            + "', releasedate:'" + album.releasedate
                                                            + "',price:'" + album.price + "',length:'" + album.length + "',studio:'" + album.studio + "',picture:'" + album.picture + "', link:'" + album.link + "'}) return n", test, Neo4jClient.Cypher.CypherResultMode.Set);
            List<Album> albums = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(response).ToList();

            /*var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Album { name: '" + album.name + "' }),(g:Author { nickname: '" + nickname + "' }) MERGE (a)-[r:ALBUM_AUTHOR]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Album responsealbum = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew).FirstOrDefault();
            var reponsenewnew1 = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Album { name: '" + album.name + "' }),(g:Genre { name: '" + genreName + "' }) MERGE (a)-[r:ALBUM_GENRE]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Album responsealbum1 = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew1).FirstOrDefault();*/
            return Ok(albums);
        }

        [HttpPost("Connect/{name}/{nickname}/{genreName}")]
        public async Task<ActionResult> ConnectAlbumAsync(string name, string nickname, string genreName)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();   

            var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Album { name: '" + name + "' }),(g:Author { nickname: '" + nickname + "' }) MERGE (a)-[r:ALBUM_AUTHOR]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Album responsealbum = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew).FirstOrDefault();

            var reponsenewnew1 = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Album { name: '" + name + "' }),(g:Genre { name: '" + genreName + "' }) MERGE (a)-[r:ALBUM_GENRE]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Album responsealbum1 = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew1).FirstOrDefault();

            return Ok(responsealbum);
        }


        [HttpDelete("Delete/{name}")]
        public async Task<ActionResult> DeleteAlbum(string name)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();
            test.Add("name", name);

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Album {name: {name}}) DETACH DELETE n", test, Neo4jClient.Cypher.CypherResultMode.Projection);

            List<Album> albums = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(response).ToList();

            return Ok(albums);
        }

        [HttpPut("Update/{name}/{price}")]
        public async Task<ActionResult> UpdateAlbum(string name, int price)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (album:Album { name: '" + name + "'}) SET album.price = '" + price + "' RETURN album", test, Neo4jClient.Cypher.CypherResultMode.Projection);

            List<Album> authors = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(response).ToList();

          

            return Ok(authors);

        }
    }
}
