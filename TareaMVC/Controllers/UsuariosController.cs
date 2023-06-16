using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TareaMVC.Models;
using TareaMVC.Services;

namespace TareaMVC.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly AplicationDbContext Context;

        public UsuariosController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, AplicationDbContext context
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.Context = context;
        }

        [AllowAnonymous]
        public IActionResult Registro() => View(new RegistroViewModel());
        

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var usuario = new IdentityUser() { Email = modelo.Email, UserName = modelo.Email };
            var resultado = await userManager.CreateAsync(usuario, modelo.Password);

            if(resultado.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach(var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(modelo);
            }
        }

        [AllowAnonymous]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var resultado = await signInManager.PasswordSignInAsync(modelo.Email, modelo.PassWord, modelo.Recuerdame, lockoutOnFailure: false);

            if (resultado.Succeeded)
                return RedirectToAction("Index", "Home");
            else
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o password incorecctos");
                return View(modelo);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<ViewResult> ListadoUsuario(string msg = null)
        {
            var usuarios = Context.Users.Select(U => new UsuarioViewModel { Email = U.Email}).ToList();

            var modelo = new UsuariosListadoViewModel()
            {
                Usuarios = usuarios, 
                Mensaje = msg
            };
            return View(modelo);
        }

        [HttpPost]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> HacerAdmin(string email)
        {
            var usuario = await Context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();

            if (usuario is null)
                return NotFound();

            await userManager.AddToRoleAsync(usuario, Constantes.RolAdmin);

            return RedirectToAction("ListadoUsuario", routeValues: new { msg = "Rol asignado correctamente a " + email });
        }


        [HttpPost]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> RemoverAdmin(string email)
        {
            var usuario = await Context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();

            if (usuario is null)
                return NotFound();

            await userManager.RemoveFromRoleAsync(usuario, Constantes.RolAdmin);
            return RedirectToAction("ListadoUsuario", routeValues: new { msg = "Rol Removido correctamente a " + email });
        }
    }
}
