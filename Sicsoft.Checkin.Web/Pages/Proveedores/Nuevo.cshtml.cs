using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GestionGastos20.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;


namespace GestionGastos20.Pages.Proveedores
{
    public class NuevoModel : PageModel
    {
        private readonly ICrudApi<ProveedoresViewModel, int> service;

        [BindProperty]
        public ProveedoresViewModel Objeto { get; set; }

        [BindProperty]
        public string Pais { get; set; }
        public NuevoModel(ICrudApi<ProveedoresViewModel, int> service)
        {
            this.service = service;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "27").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                var Pais = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Pais").Select(s1 => s1.Value).FirstOrDefault();

                this.Pais = Pais;
                return Page();
            }
            catch (ApiException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(Objeto.Nombre))
                {
                    throw new Exception("El nombre debe contener elementos");
                }

                if (string.IsNullOrEmpty(Objeto.RUC))
                {
                    throw new Exception("El RUC debe contener elementos");

                }
                // Objeto.idLogin = int.Parse(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.Actor).Select(s1 => s1.Value).FirstOrDefault());
                await service.Agregar(Objeto);
                return RedirectToPage("./Index");
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
            catch (Exception ex)
            { 
                ModelState.AddModelError(string.Empty, ex.Message);

                return Page();
            }
        }
    }
}
