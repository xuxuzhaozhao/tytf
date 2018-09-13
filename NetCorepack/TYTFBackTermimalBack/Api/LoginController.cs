using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TYTFBackTermimalBack.Models;

namespace TYTFBackTermimalBack.Api
{
    [Produces("application/json")]
    [Route("api/User")]
    public class LoginController : Controller
    {
        [HttpPost, Route("login")]
        public object Login([FromBody]TempLoginClass user)
        {
            var result = new XResult();
            if (user.UserName == "xuhong" && user.Password == "xuhongadmin")
            {
                result.data = "xuhong";
            }
            else
            {
                result.code = 10000;
            }
            return result;
        }
    }
    public class TempLoginClass
    {
        [JsonProperty("username")]
        public string UserName { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}