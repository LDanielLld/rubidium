using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace REVIREPanels.Modelos
{
    public class Usuario
    {
        public enum Sexo
        {
            Nothing, Male, Female
        }

        public enum Lesion
        {
            Nothing, MSD, MSI
        }
        public int UsuarioID { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }        
        public int Edad { get; set; }        
        public Sexo Genero { get; set; }        
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaDiagnostico { get; set; }
        public Lesion Lateralidad { get; set; }
        public string Imagen { get; set; }
        public bool Espasticidad { get; set; }
        public string Comentarios { get; set; }
        public bool Antiguo { get; set; }

       // public virtual ICollection<Rutina> Rutinas { get; set; }
      //  public virtual ICollection<Chat> Chats { get; set; }

    }
}
