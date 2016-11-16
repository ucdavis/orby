using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Threading;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace orby.Controllers
{
    [Route("api/[controller]")]
    public class ColorController : Controller
    {
        private readonly AppSettings _appSettings;

        public static string CurrentColor = "#ff0000 ";
        public static Timer Timer;
        public static int k = 0; // For counting Timer color changes

        public ColorController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            if (Timer == null)
            {
                // Initialize Timer to not fire until we get a pending webhook
                Timer = new Timer(TimerCallback, "state", -1, -1);
            }
        }

        private void TimerCallback(object state)
        {
            if (k % 2 == 0)
            {
                Get("A19825");
            }
            else
            {
                Get("FFEE00");
            }
            k++;            
        }


        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<string> Get(string id)
        {
            var newColor = "#" + id;
            var json = new { program = "Demo", color = newColor };
            var jsonString = JsonConvert.SerializeObject(json);

            var client = new HttpClient();
            var result = await client.PostAsync( _appSettings.OrbID, new StringContent(jsonString, Encoding.UTF8, "application/json"));

            return id;
        }

        // POST api/values
        [HttpPost("default")]
        public async void Post([FromBody]GitHubResponse response)
        {
            if (response.action == "opened")
            {
                await Get("FF0000");
            } else
            {
                await Get("0000FF");
            }
        }

        [HttpPost("status")]
        public async void Post([FromBody]StatusEvent response)
        {
            if (response.state == "success")
            {
                Timer.Change(-1, -1);
                await Get("4ef442");
            }
            else if (response.state == "pending")
            {
                Timer.Change(0, 1000);
            }
            else //Must be failure or error
            {
                Timer.Change(-1, -1);
                await Get("cc0000");
            }
            
        }

        [HttpPost("pr")]
        public async void Post([FromBody]PullRequestReviewEvent response)
        {
            if (response.review.state == "approved")
            {
                Timer.Change(-1, -1);
                await Get("4ef442");
            }
            else if (response.review.state == "pending")
            {
                Timer.Change(0, 1000);
            }
            else
            {
                Timer.Change(-1, -1);
                await Get("cc0000");
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class StatusEvent
    {
        public int id { get; set; }
        public string name { get; set; }
        //Can be pending, success, failure, or error
        public string state { get; set; }
    }

    public class PullRequestReviewEvent
    {
        public string action { get; set; }
        public ReviewContainer review { get; set; }
        public class ReviewContainer
        {
            public string state { get; set; }
            public string body { get; set; }
            public UserContainer user { get; set; }
            public class UserContainer
            {
                public string login { get; set; }
            }
        }
        
    }

    public class GitHubResponse
    {
        public string action { get; set; }
        public IssueContainer issue { get; set; }
        public RepositoryContainer repository { get; set; }
        public SenderContainer sender { get; set; }
        public class IssueContainer
        {
            public string url { get; set; }
            public int number { get; set; }
        }
        public class RepositoryContainer
        {
            public int id { get; set; }
            public string full_name { get; set; }
            public OwnerContainer owner { get; set; }
        }
        public class SenderContainer
        {
            public string login { get; set; }
            public int id { get; set; }
        }
        public class OwnerContainer
        {
            public string login { get; set; }
            public int id { get; set; }
        }
    }
}
