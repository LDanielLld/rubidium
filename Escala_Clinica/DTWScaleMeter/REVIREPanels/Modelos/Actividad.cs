using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REVIREPanels.Modelos
{
    public class Actividad
    {
        public enum Dia
        {
            Nothing, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
        }


        public enum TypeAsistencia
        {
            Nothing, AdjustResisted, Resisted, Free, AdjustAssisted, Assisted
        }

        public enum Orden
        {
            Random, Clockwise, AntiClockwise, Nothing
        }

        public int ActividadID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Fecha { get; set; }
        public string Nombre { get; set; }
       // public Dia DiaActividad { get; set; }
        public string TipoActividad { get; set; }
        public TypeAsistencia TipoAsistencia { get; set; }
        public int Nivel { get; set; }
        public int Minutos { get; set; }
        public int Segundos { get; set; }
        public int Repeticiones { get; set; }
        public int TiempoObjetivo { get; set; }
        public int Distancia { get; set; }
        public Orden OrdenObjetivo { get; set; }
       // public string Imagen { get; set; }
       // public int Aciertos { get; set; }
       // public List<bool> ActInterface { get; set; }
       // public string NameAuxiliar { get; set; }        
        public string DatosAuxiliares{get; set;}
        public string DatosRecibidos { get; set; }
        //public virtual ICollection<Rutina> Rutinas { get; set; }
    }
}
