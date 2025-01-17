﻿using HSMCommon;
using HSMCommon.Constants;
using HSMServer.Attributes;
using HSMServer.Authentication;
using HSMServer.Constants;
using HSMServer.Configuration;
using HSMServer.Encryption;
using HSMServer.Registration;
using HSMServer.Filters;
using HSMServer.Model.Authentication;
using HSMServer.Model.TreeViewModel;
using HSMServer.Model.Validators;
using HSMServer.Model.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HSMServer.Controllers
{
    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class AccountController : Controller
    {
        private readonly IUserManager _userManager;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IRegistrationTicketManager _ticketManager;
        private readonly TreeViewModel _treeViewModel;


        public AccountController(IUserManager userManager, IConfigurationProvider configurationProvider,
            IRegistrationTicketManager ticketManager, TreeViewModel treeViewModel)
        {
            _userManager = userManager;
            _configurationProvider = configurationProvider;
            _ticketManager = ticketManager;
            _treeViewModel = treeViewModel;
        }

        #region Login

        [AllowAnonymous]
        [UnauthorizedAccessOnlyFilter]
        [ActionName(nameof(Index))]
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [Consumes("application/x-www-form-urlencoded")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Authenticate([FromForm] LoginViewModel model)
        {
            LoginValidator validator = new(_userManager);
            var results = validator.Validate(model);
            if (!results.IsValid)
            {
                TempData[TextConstants.TempDataErrorText] = ValidatorHelper.GetErrorString(results.Errors);
                return RedirectToAction("Index", "Home");
            }

            TempData.Remove(TextConstants.TempDataErrorText);
            await Authenticate(model.Username, model.KeepLoggedIn);

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Registration

        [AllowAnonymous]
        [UnauthorizedAccessOnlyFilter]
        public IActionResult Registration([FromQuery(Name = "Cipher")] string cipher,
            [FromQuery(Name = "Tag")] string tag, [FromQuery(Name = "Nonce")] string nonce)
        {
            var model = new RegistrationViewModel();

            if (!string.IsNullOrEmpty(cipher) && !string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(nonce))
            {
                var key = _configurationProvider.ReadConfigurationObject(ConfigurationConstants.AesEncryptionKey);
                byte[] keyBytes = AESCypher.ToBytes(key.Value);

                var result = AESCypher.Decrypt(cipher.Replace(' ', '+'), nonce.Replace(' ', '+'), tag.Replace(' ', '+'), keyBytes);
                var ticketId = Guid.Parse(result);
                var ticket = _ticketManager.GetTicket(ticketId);
                if (ticket == null)
                {
                    return RedirectToAction("Index", "Error", new ErrorViewModel()
                    {
                        ErrorText = "Link already used.",
                        StatusCode = "500"
                    });
                }

                if (ticket.ExpirationDate < DateTime.UtcNow)
                    return RedirectToAction("Index", "Error", new ErrorViewModel()
                    {
                        ErrorText = "Link expired.",
                        StatusCode = "500"
                    });

                model.ProductKey = ticket.ProductKey;
                model.Role = ticket.Role;
                model.TicketId = ticket.Id.ToString();
            }

            return View(model);
        }

        [AllowAnonymous]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Registrate([FromForm] RegistrationViewModel model)
        {
            RegistrationValidator validator = new(_userManager);
            var results = validator.Validate(model);
            if (!results.IsValid)
            {
                TempData[TextConstants.TempDataErrorText] = ValidatorHelper.GetErrorString(results.Errors);
                return View("Registration", model);
            }

            List<(Guid, ProductRoleEnum)> products = null;
            if (!string.IsNullOrEmpty(model.ProductKey) && !string.IsNullOrEmpty(model.Role))
            {
                products = new List<(Guid, ProductRoleEnum)>()
                {
                    (Guid.Parse(model.ProductKey), (ProductRoleEnum)int.Parse(model.Role))
                };
            }

            await _userManager.AddUser(model.Username, HashComputer.ComputePasswordHash(model.Password), false, products);
            await Authenticate(model.Username, true);

            if (!string.IsNullOrEmpty(model.TicketId))
                _ticketManager.RemoveTicket(Guid.Parse(model.TicketId));

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Users

        [AuthorizeIsAdmin(true)]
        public IActionResult Users()
        {
            // TODO: use ViewComponent and remove using TempData for passing products
            TempData[TextConstants.TempDataProductsText] =
                _treeViewModel.Nodes.Values.ToDictionary(product => product.Id, product => product.Name);

            var users = _userManager.GetUsers().OrderBy(x => x.Name);
            return View(users.Select(x => new UserViewModel(x)).ToList());
        }

        [HttpPost]
        public Task RemoveUser([FromBody] UserViewModel model)
        {
            return _userManager.RemoveUser(model.Username);
        }

        [HttpPost]
        public async Task CreateUser([FromBody] UserViewModel model)
        {
            NewUserValidator validator = new(_userManager);
            var results = validator.Validate(model);

            if (!results.IsValid)
                TempData[TextConstants.TempDataErrorText] = ValidatorHelper.GetErrorString(results.Errors);
            else
                await _userManager.AddUser(model.Username.Trim(), HashComputer.ComputePasswordHash(model.Password), model.IsAdmin);
        }

        [HttpPost]
        public Task UpdateUser([FromBody] UserViewModel userViewModel)
        {
            if (_userManager.TryGetIdByName(userViewModel.Username, out var userId))
            {
                var updateUser = new UserUpdate
                {
                    Id = userId,
                    IsAdmin = userViewModel.IsAdmin,
                };

                return _userManager.TryUpdate(updateUser);
            }

            return Task.CompletedTask;
        }

        #endregion

        public async Task<IActionResult> Logout()
        {
            TempData.Remove(TextConstants.TempDataErrorText);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        private Task Authenticate(string login, bool keepLoggedIn)
        {
            var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, login) };
            ClaimsIdentity id = new(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            AuthenticationProperties properties = new() { IsPersistent = keepLoggedIn };
            return HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id), properties);
        }
    }
}
