using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REVIREPanels.Modelos
{
    public class Sesion
    {        
        public int SesionID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Fecha { get; set; }
        public string Comentarios { get; set; }
        
        //public virtual ICollection<Rutina> Rutinas { get; set; }
    }
}
