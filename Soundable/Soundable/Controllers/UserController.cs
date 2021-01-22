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
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IGraphClient _client;


        public UserController(ILogger<UserController> logger, IGraphClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet("Get/{username}")]
        public async Task<ActionResult> GetAsyncc(string username)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH(n:User)WHERE n.username <> 'admin' AND  n.username <>  '" + username + "' return n", test, Neo4jClient.Cypher.CypherResultMode.Set);
            List<User> users = ((IRawGraphClient)_client).ExecuteGetCypherResults<User>(response).ToList();

          
            

            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult> GetAsync(string username)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:User) - [IS_FRIEND] - (user:User { username: '" + username + "'}) RETURN n", test, Neo4jClient.Cypher.CypherResultMode.Set);
            List<User> users = ((IRawGraphClient)_client).ExecuteGetCypherResults<User>(response).ToList();

            foreach (var u in users)
            {
                Dictionary<string, object> test1 = new Dictionary<string, object>();
                test1.Add("name", u.username);
                var responsenew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Playlist) <-[MY_PLAYLIST] - (user:User{ username: {name} }) RETURN n", test1, Neo4jClient.Cypher.CypherResultMode.Set);
                List<Playlist> playlists = ((IRawGraphClient)_client).ExecuteGetCypherResults<Playlist>(responsenew).ToList();
                u.playlists = playlists;
                foreach (var p in playlists)
                {
                    var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Playlist{ name: '" + p.name + "'}) - [PLAYLIST_ALBUM] -> (album:Album ) RETURN album", test1, Neo4jClient.Cypher.CypherResultMode.Set);
                    List<Album> albums = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew).ToList();
                    p.albums = albums;
                }
                /*var reponsenewnew1 = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:Genre)< - [ALBUM_GENRE] - (album:Album { name: {name} }) RETURN n", test1, Neo4jClient.Cypher.CypherResultMode.Set);
                Genre genre = ((IRawGraphClient)_client).ExecuteGetCypherResults<Genre>(reponsenewnew1).FirstOrDefault();
                a.genre = genre;*/
            }

            return Ok(users);
        }

        [HttpPost("Create")]
        public async Task<ActionResult> CreateUserAsync([FromBody] User user)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var response = new Neo4jClient.Cypher.CypherQuery("CREATE (n:User { name:'" + user.name + "', surname:'" + user.surname + "',username:'" + user.username + "' , password:'" + user.password + "' }) return n", test, Neo4jClient.Cypher.CypherResultMode.Set);
            List<User> users = ((IRawGraphClient)_client).ExecuteGetCypherResults<User>(response).ToList();

            /*var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Album { name: '" + album.name + "' }),(g:Author { nickname: '" + nickname + "' }) MERGE (a)-[r:ALBUM_AUTHOR]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Album responsealbum = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew).FirstOrDefault();

            var reponsenewnew1 = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Album { name: '" + album.name + "' }),(g:Genre { name: '" + genreName + "' }) MERGE (a)-[r:ALBUM_GENRE]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Album responsealbum1 = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew1).FirstOrDefault();*/


            return Ok(users);
        }

        [HttpDelete("Delete/{username}")]
        public async Task<ActionResult> DeleteUser(string username)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();
            test.Add("username", username);

            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (n:User {username: {username}}) DETACH DELETE n", test, Neo4jClient.Cypher.CypherResultMode.Projection);

            List<User> users = ((IRawGraphClient)_client).ExecuteGetCypherResults<User>(response).ToList();

            return Ok(users);
        }

        [HttpPut("Update/{Username}/{NewUsername}")]
        public async Task<ActionResult> UpdateUser(string Username, string NewUsername)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();


            var response = new Neo4jClient.Cypher.CypherQuery(@"MATCH (user:User { username: '" + Username + "'}) SET user.username = '" + NewUsername + "' RETURN user", test, Neo4jClient.Cypher.CypherResultMode.Projection);

            List<User> users = ((IRawGraphClient)_client).ExecuteGetCypherResults<User>(response).ToList();

            return Ok(users);

        }

        [HttpPost("Login")]
        public async Task<ActionResult> LoginUserAsync([FromBody] User user)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var response = new Neo4jClient.Cypher.CypherQuery("MATCH (user:User { username: '" + user.username + "', password: '" + user.password + "'})  RETURN user", test, Neo4jClient.Cypher.CypherResultMode.Set);
            List<User> users = ((IRawGraphClient)_client).ExecuteGetCypherResults<User>(response).ToList();

            /*var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Album { name: '" + album.name + "' }),(g:Author { nickname: '" + nickname + "' }) MERGE (a)-[r:ALBUM_AUTHOR]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Album responsealbum = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew).FirstOrDefault();

            var reponsenewnew1 = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:Album { name: '" + album.name + "' }),(g:Genre { name: '" + genreName + "' }) MERGE (a)-[r:ALBUM_GENRE]->(g) return a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            Album responsealbum1 = ((IRawGraphClient)_client).ExecuteGetCypherResults<Album>(reponsenewnew1).FirstOrDefault();*/

                
            return Ok(users);
        }

        [HttpPost("Connect/{username}")]
        public async Task<ActionResult> ConnectUserAsync([FromBody] User user, string username)
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            var reponsenewnew = new Neo4jClient.Cypher.CypherQuery(@"MATCH (a:User),(b:User) WHERE a.username = '" + user.username + "' AND b.username = '" + username + "' CREATE (a)-[r:IS_FRIEND]->(b) RETURN a", test, Neo4jClient.Cypher.CypherResultMode.Set);
            User user1 = ((IRawGraphClient)_client).ExecuteGetCypherResults<User>(reponsenewnew).FirstOrDefault();




            return Ok(user1);
        }

    }
}
