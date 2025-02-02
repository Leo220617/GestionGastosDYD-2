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

namespace GestionGastos20.Pages.Liquidaciones
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<ListadoCierresViewModel, int> service;
        private readonly ICrudApi<UsuariosViewModel, int> users;
        private readonly ICrudApi<RolesViewModel, int> roles;
        private readonly ICrudApi<DevolucionSAP, int> sap;

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        [BindProperty]
        public ListadoCierresViewModel[] Objeto { get; set; }

        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }


        [BindProperty]
        public RolesViewModel[] Rols { get; set; }

        [BindProperty]
        public string Pais { get; set; }

        public IndexModel(ICrudApi<ListadoCierresViewModel, int> service, ICrudApi<UsuariosViewModel, int> users, ICrudApi<RolesViewModel, int> roles, ICrudApi<DevolucionSAP, int> sap)
        {
            this.service = service;
            this.users = users;
            this.roles = roles;
            this.sap = sap;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "4").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }
                Pais = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Pais").Select(s1 => s1.Value).FirstOrDefault();

                var Periodos = new string[12] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Setiembre", "Octubre", "Noviembre", "Diciembre" };
                DateTime time = new DateTime();

                if (time == filtro.FechaInicio)
                {


                    filtro.FechaInicio = DateTime.Now;

                    filtro.FechaInicio = new DateTime(filtro.FechaInicio.Year, filtro.FechaInicio.Month, 1);


                    DateTime primerDia = new DateTime(filtro.FechaInicio.Year, filtro.FechaInicio.Month, 1);


                    DateTime ultimoDia = primerDia.AddMonths(1).AddDays(-1);

                    filtro.FechaFinal = ultimoDia;



                }








                Rols = await roles.ObtenerLista("");
                var MiRol = int.Parse(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.Role).Select(s1 => s1.Value).FirstOrDefault());



                var Rol = Rols.Where(a => a.NombreRol.ToUpper().Contains("Administrador".ToUpper())).FirstOrDefault();




                Usuarios = await users.ObtenerLista("");
                var idUsuarioAsignacion = int.Parse(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == ClaimTypes.Actor).Select(s1 => s1.Value).FirstOrDefault());



                //if (Rol.idRol != MiRol)
                if (string.IsNullOrEmpty(Roles.Where(a => a == "22").FirstOrDefault()))
                {
                    Usuarios = Usuarios.Where(a => a.idLoginAceptacion == idUsuarioAsignacion).ToArray();
                    filtro.Codigo2 = idUsuarioAsignacion;

                }
                else
                {
                    filtro.Codigo2 = 0;
                }

                Objeto = await service.ObtenerLista(filtro);


                return Page();
            }
            catch (Exception ex)
            {
                // Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, ex.Message);

                return Page();
            }
        }

        public async Task<IActionResult> OnGetEstado(int idB)
        {
            try
            {
                await service.CambiaEstado(idB, "P", "");

                return new JsonResult(true);
            }
            catch (Exception ex)
            {



                return new JsonResult(ex.Message);
            }
        }

        public async Task<IActionResult> OnGetAsiento(int idB)
        {
            try
            {
                var resp = await sap.InsertarAsiento(idB);


                return new JsonResult(resp);
            }
            catch (Exception ex)
            {



                return new JsonResult(ex.Message);
            }
        }
    }
}
