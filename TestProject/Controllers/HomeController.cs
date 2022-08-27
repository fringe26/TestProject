using LoginService;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestProject.Models;
using System.Net;

namespace TestProject.Controllers
{
    public class HomeController : Controller
    {

        private readonly IICUTech _soapService;
        private IHttpContextAccessor _accessor;
        public HomeController(IICUTech soapService, IHttpContextAccessor accessor)
        {
            _soapService = soapService;
            _accessor = accessor;
        }

        [HttpGet]
        public IActionResult Index()
        {
            
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginVM loginVM)
        {

            if (!ModelState.IsValid) return View();

            LoginResponse loginResponse = await _soapService.LoginAsync(new LoginRequest
            {
                UserName = loginVM.Email,
                Password = loginVM.Password,
                IPs = GetIpAdress()
            });

            if (loginResponse.@return.Contains("EntityId"))
            {
                return RedirectToAction("Success");
            }
            else
            {
                return RedirectToAction("Error");
            }
            
            
        }

        public IActionResult Success()
        {
            
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }


        public string GetIpAdress()
        {
            IPAddress remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            string result = "";
            if (remoteIpAddress != null)
            {
                // If we got an IPV6 address, then we need to ask the network for the IPV4 address 
                // This usually only happens when the browser is on the same machine as the server.
                if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    remoteIpAddress = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList
            .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }
                result = remoteIpAddress.ToString();
            }
            return result;
        }
    }
}
