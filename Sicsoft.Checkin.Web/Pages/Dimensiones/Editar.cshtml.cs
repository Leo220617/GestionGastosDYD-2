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


namespace GestionGastos20.Pages.Dimensiones
{
    public class EditarModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<DimensionesViewModel, int> service;

        [BindProperty]
        public DimensionesViewModel Objeto { get; set; }


        public EditarModel(ICrudApi<DimensionesViewModel, int> service)
        {
            this.service = service;

        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "19").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                int ids = Convert.ToInt32(id);

                Objeto = await service.ObtenerPorId(ids);

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

                await service.Editar(Objeto);
                return RedirectToPage("./Index");
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }
    }
}
