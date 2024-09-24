namespace GestionGastos20.Models
{
    public class DetCierreViewModel
{
        public int id { get; set; }

        public int idCierre { get; set; }

        public int NumLinea { get; set; }

        public int idFactura { get; set; }
        public int idTipoGasto { get; set; }
        public string Comentario { get; set; }
    }
}
