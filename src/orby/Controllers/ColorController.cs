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

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace orby.Controllers
{
    [Route("api/[controller]")]
    public class ColorController : Controller
    {
        private readonly AppSettings _appSettings;

        public ColorController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
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
        [HttpPost]
        public async void Post([FromBody]GitHubResponse response)
        {
            if (response.action == "opened")
            {
                await Get("FF0000");
            } else {
                await Get("0000FF");
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
