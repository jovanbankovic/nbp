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
    public class PlaylistController : Controller
    {

        private readonly ILogger<PlaylistController> _logger;
        private readonly IGraphClient _client;


        public PlaylistController(ILogger<PlaylistController> logger, IGraphClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult> GetAsync(string username)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Playlist) <-[MY_PLAYLIST] - (user:User { username: '" + username + "'}) RETURN n", test, Neo4jClient.Cypher.CypherResultMode.Set);
            List<Playlist> playlists = ((IRawGraphClient)_client).ExecuteGetCypherResults<Playlist>(response).ToList();

            foreach (var p in playlists)
            {
                Dictionary<string, object> test1 = new Dictionary<string, object>();
                test1.Add("name", p.name);
                var responsenew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Playlist{ name: {name} }) <-[MY_PLAYLIST] - (user:User) RETURN user", test1, Neo4jClient.Cypher.CypherResultMode.Set);
                User user = ((IRawGraphClient)_client).ExecuteGetCypherResults<User>(responsenew).FirstOrDefault();
                p.user = user;

                 var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Playlist{ name: {name} }) - [PLAYLIST_ALBUM] -> (album:Album ) RETURN album", test1, Neo4jClient.Cypher.CypherResultMode.Set);
                 List<Album> albums = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew).ToList();
                 p.albums = albums;

                 /*var reponsenewnew1 = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Genre)< - [ALBUM_GENRE] - (album:Album { name: {name} }) RETURN n", test1, Neo4jClient.Cypher.CypherResultMode.Set);
                 Genre genre = ((IRawGraphClient)_client).ExecuteGetCypherResults<Genre>(reponsenewnew1).FirstOrDefault();
                 a.genre = genre;*/
            }

            return Ok(playlists);
        }

        [HttpDelete("Delete/{name}")]
        public async Task<ActionResult> DeletePlaylist(string name)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();
            test.Add("name", name);

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Playlist {name: {name}}) DETACH DELETE n", test, Neo4jClient.Cypher.CypherResultMode.Projection);

            List<Playlist> playlists = ((IRawGraphClient)_client).ExecuteGetCypherResults<Playlist>(response).ToList();

            return Ok(playlists);
        }

        [HttpPost("Create/{username}")]
        public async Task<ActionResult> CreatePlaylistAsync([FromBody] Playlist playlist, string username)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var response = new Neo4jClient.Cypher.CypherQuery("CREATE (n:Playlist { name:'" + playlist.name + "' }) return n", test, Neo4jClient.Cypher.CypherResultMode.Set);
            List<Playlist> playlists = ((IRawGraphClient)_client).ExecuteGetCypherResults<Playlist>(response).ToList();

            var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Playlist { name: '" + playlist.name + "' }),(g:User { username: '" + username + "' }) MERGE (a)<-[r:MY_PLAYLIST]-(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Playlist responsealbum = ((IRawGraphClient)_client).ExecuteGetCypherResults<Playlist>(reponsenewnew).FirstOrDefault();

            /*var reponsenewnew1 = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Album { name: '" + album.name + "' }),(g:Genre { name: '" + genreName + "' }) MERGE (a)-[r:ALBUM_GENRE]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Album responsealbum1 = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew1).FirstOrDefault();*/


            return Ok(playlists);
        }

         [HttpPost("Connect/{AlbumName}/{PlaylistName}")]
        public async Task<ActionResult> ConnectPlaylistAsync(string AlbumName, string PlaylistName)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Playlist { name: '" + PlaylistName + "' }),(g:Album { name: '" + AlbumName + "' }) MERGE (a)-[r:PLAYLIST_ALBUM]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Playlist responseplaylist = ((IRawGraphClient)_client).ExecuteGetCypherResults<Playlist>(reponsenewnew).FirstOrDefault();

            return Ok(responseplaylist);
        }

        [HttpDelete("DeleteAlbumInPlaylist/{PlaylistName}/{AlbumName}")]
        public async Task<ActionResult> DeleteAlbumInPlaylist(string PlaylistName, string AlbumName)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();
            test.Add("PlaylistName", PlaylistName);
            test.Add("AlbumName", AlbumName);

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Playlist {name: {PlaylistName}})-[r:PLAYLIST_ALBUM]->(g:Album { name: '" + AlbumName + "' })  DELETE r", test, Neo4jClient.Cypher.CypherResultMode.Projection);

            List<Playlist> playlists = ((IRawGraphClient)_client).ExecuteGetCypherResults<Playlist>(response).ToList();

            return Ok(playlists);
        }

        [HttpPut("Update/{PlaylistName}/{NewPlaylistName}")]
        public async Task<ActionResult> UpdatePlaylist(string PlaylistName, string NewPlaylistName)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();


            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (playlist:Playlist { name: '" + PlaylistName + "'}) SET playlist.name = '" + NewPlaylistName + "' RETURN playlist", test, Neo4jClient.Cypher.CypherResultMode.Projection);

            Playlist playlist = ((IRawGraphClient)_client).ExecuteGetCypherResults<Playlist>(response).FirstOrDefault();

            return Ok(playlist);

        }

    }
}
