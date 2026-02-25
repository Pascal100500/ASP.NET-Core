// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace GamePortal.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<IdentityUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            var email = User.Identity?.Name;// Ввожу переменную для отображения UserName. По умолчаеию сейчас Identity использует Email
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknow"; // IP может быть null!!!
            await _signInManager.SignOutAsync();
            //_logger.LogInformation("User logged out.");
            using (LogContext.PushProperty("LogType", "User"))
            {
                _logger.LogInformation("Пользователь {Email} вышел из системы с IP {IP}", email, ip); // мой логер
            }
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage();
            }
        }
    }
}
