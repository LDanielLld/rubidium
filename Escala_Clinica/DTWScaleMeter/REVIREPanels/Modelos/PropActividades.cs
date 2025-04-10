using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REVIREPanels.Modelos
{
    public class PropActividades
    {
        /*public enum Categoria{
            NONE,
            MOTOR,
            COGNITIVO
        }*/

        public int ActividadID { get; set; }
        public string Nombre { get; set; }
        public List<bool> ActInterface { get; set; }
        public string Fondo { get; set; }
        public string Icono { get; set; }
        public string Tipo { get; set; }
        public string NameAuxiliar { get; set; }

        public bool Asistido { get; set; }
        public bool AsistidoReg { get; set; }
        public bool Libre { get; set; }
        public bool ResistidoReg { get; set; }
        public bool Resistido { get; set; }
        //public Categoria nCategoria { get; set; }
        public string nCategoria { get; set; }
        public string NameTableInfo { get; set; }

        public PropActividades Clone()
        {
            return this.MemberwiseClone() as PropActividades;
        }

        public List<bool> CloneList()
        {
            return ActInterface.GetRange(0, ActInterface.Count);
        }
    }
}
