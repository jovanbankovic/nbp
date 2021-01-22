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
    public class SongController : Controller
    {
        private readonly ILogger<SongController> _logger;
        private readonly IGraphClient _client;

        public SongController(ILogger<SongController> logger, IGraphClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Song) RETURN n", test, Neo4jClient.Cypher.CypherResultMode.Set);
            List<Song> songs = ((IRawGraphClient)_client).ExecuteGetCypherResults<Song>(response).ToList();

            foreach (var a in songs)
            {
                Dictionary<string, object> test1 = new Dictionary<string, object>();
                test1.Add("name", a.name);

                var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Album) - [ALBUM_SONG] - >(song:Song { name: {name} }) RETURN n", test1, Neo4jClient.Cypher.CypherResultMode.Set);
                Album album = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew).FirstOrDefault();
                a.album = album;

                var responsenew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Awards)< - [SONG_AWARD] - (song:Song { name: {name} }) RETURN n", test1, Neo4jClient.Cypher.CypherResultMode.Set);
                List<Awards> awards = ((IRawGraphClient)_client).ExecuteGetCypherResults<Awards>(responsenew).ToList();
                a.awards = awards;
            }

            return Ok(songs);
        }
        [HttpPost("Create")]
        public async Task<ActionResult> CreateSongAsync([FromBody] Song song)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var response = new Neo4jClient.Cypher.CypherQuery("CREATE (n:Song { name:'" + song.name
                                                            + "', duration:'" + song.duration
                                                            + "',released:'" + song.released + "'}) return n", test, Neo4jClient.Cypher.CypherResultMode.Set);
            List<Song> songs = ((IRawGraphClient)_client).ExecuteGetCypherResults<Song>(response).ToList();

            /*var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Album { name: '" + album.name + "' }),(g:Author { nickname: '" + nickname + "' }) MERGE (a)-[r:ALBUM_AUTHOR]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Album responsealbum = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew).FirstOrDefault();

            var reponsenewnew1 = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Album { name: '" + album.name + "' }),(g:Genre { name: '" + genreName + "' }) MERGE (a)-[r:ALBUM_GENRE]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Album responsealbum1 = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew1).FirstOrDefault();*/


            return Ok(songs);
        }

        [HttpPost("Connect/{SongName}/{AlbumName}")]
        public async Task<ActionResult> ConnectSongAsync(string SongName, string AlbumName)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Album { name: '" + AlbumName + "' }),(g:Song { name: '" + SongName + "' }) MERGE (a)-[r:ALBUM_SONG]->(g) return g", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Song song = ((IRawGraphClient)_client).ExecuteGetCypherResults<Song>(reponsenewnew).FirstOrDefault();

            /*var reponsenewnew1 = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Author { nickname: '" + AuthorNickname + "' }),(g:Awards { name: '" + AwardName + "' }) MERGE (a)-[r:AUTHOR_AWARDS]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Author responsealbum1 = ((IRawGraphClient)_client).ExecuteGetCypherResults<Author>(reponsenewnew1).FirstOrDefault();*/


            return Ok(song);
        }

        [HttpDelete("Delete/{SongName}")]
        public async Task<ActionResult> DeleteLabel(string SongName)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();
            test.Add("name", SongName);

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Song {name: {name}}) DETACH DELETE n", test, Neo4jClient.Cypher.CypherResultMode.Projection);

            List<Song> songs = ((IRawGraphClient)_client).ExecuteGetCypherResults<Song>(response).ToList();

            return Ok(songs);
        }

        [HttpPut("Update/{SongName}/{NewSongName}")]
        public async Task<ActionResult> UpdateLabel(string SongName, string NewSongName)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();


            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (song:Song { name: '" + SongName + "'}) SET song.name = '" + NewSongName + "' RETURN song", test, Neo4jClient.Cypher.CypherResultMode.Projection);

            List<Song> songs = ((IRawGraphClient)_client).ExecuteGetCypherResults<Song>(response).ToList();

            return Ok(songs);

        }
    }
}
