using Autism_Video_API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Autism_Video_API.Controllers
{
    public class MediaServiceRegisterController : ApiController
    {
        public void Post([FromBody] PathfinderVideo video, string url)
        {
            video.RegisterMediaServiceUrl(GetStorConnStr(), url);
        }

        private string GetStorConnStr()
        {
            return ConfigurationManager.AppSettings["StorageConnectionString"];
        }


    }
}