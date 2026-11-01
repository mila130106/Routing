using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProductsWithRouting.Models;
using ProductsWithRouting.Services;

namespace ProductsWithRouting.Controllers
{
    public class UsersController : Controller
    {
        private const string AdminKey = "df2323eoT";

        private readonly List<User> myUsers;

        public UsersController(Data data)
        {
            myUsers = data.Users;
        }

        // URL is always /users/index. The key is read ONLY from the request body (form),
        // never from route or query string.
        [HttpPost]
        public IActionResult Index([FromForm(Name = "key")] string key)
        {
            if (key != AdminKey)
            {
                return Unauthorized();
            }
            return View(myUsers);
        }

        // A plain GET has no body, so there is no key -> unauthorized
        [HttpGet]
        public IActionResult Index()
        {
            return Unauthorized();
        }
    }
}