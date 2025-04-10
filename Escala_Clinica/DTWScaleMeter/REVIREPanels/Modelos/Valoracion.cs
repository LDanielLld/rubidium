using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REVIREPanels.Modelos
{
    class Valoracion
    {
        public int ValoracionID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Fecha { get; set; }
        public float AmplMax { get; set; }
        public float ForceMax { get; set; }

        //Amplitud
        public float AmplUp { get; set; }
        public float AmplDown { get; set; }
        public float AmplRight { get; set; }
        public float AmplLeft { get; set; }
        public float AmplTimeUp { get; set; } //Tiempos
        public float AmplTimeDown { get; set; }
        public float AmplTimeRight { get; set; }
        public float AmplTimeLeft { get; set; }

        //Fuerza
        public float ForceUp { get; set; }
        public float ForceDown { get; set; }
        public float ForceRight { get; set; }
        public float ForceLeft { get; set; }
        public float ForceAmplUp { get; set; } //Amplitud
        public float ForceAmplDown { get; set; }
        public float ForceAmplRight { get; set; }
        public float ForceAmplLeft { get; set; }
        public float ForceTimeUp { get; set; } //Tiempos
        public float ForceTimeDown { get; set; }
        public float ForceTimeRight { get; set; }
        public float ForceTimeLeft { get; set; }

    }
}
