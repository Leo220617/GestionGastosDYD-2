using System;

namespace GestionGastos20.Models
{
    public class BitacoraCierresViewModel
    {
        public int id { get; set; }
        public int idUsuarioEnviador { get; set; }
        public int idUsuarioAceptador { get; set; }
        public int idCierre { get; set; }
        public string Detalle { get; set; }
        public DateTime Fecha { get; set; }
        public string IP { get; set; }
    }
}
