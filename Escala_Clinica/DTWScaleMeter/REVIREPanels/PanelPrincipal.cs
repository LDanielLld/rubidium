using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;

namespace REVIREPanels
{
    public partial class PanelPrincipal : UserControl
    {
        public static PanelPrincipal sharedInstance = null;

        private PanelEstadisticas inicio;
        public PanelEstadisticas Inicio
        {
            get
            {
                if (inicio == null)
                {
                    inicio = new PanelEstadisticas(0);
                    inicio.Dock = DockStyle.Fill; //Para ajustar
                }
                return inicio;
            }
        }

        /*private PanelValoracion inicio;
        public PanelValoracion Inicio
        {
            get
            {
                if (inicio == null)
                {
                    inicio = new PanelValoracion();
                    inicio.Dock = DockStyle.Fill; //Para ajustar
                }
                return inicio;
            }
        }*/

        /* private PanelEstadisticas inicio;
         public PanelEstadisticas Inicio
         {
             get
             {
                 if (inicio == null)
                 {
                     inicio = new PanelEstadisticas();
                     inicio.Dock = DockStyle.Fill; //Para ajustar
                 }
                 return inicio;
             }
         }*/

       /*  private PanelProgramacion inicio;
    public PanelProgramacion Inicio
    {
        get
        {
            if (inicio == null)
            {
                inicio = new PanelProgramacion();
                inicio.Dock = DockStyle.Fill; //Para ajustar
            }
            return inicio;
        }
    }*/

        public PanelPrincipal()
        {
            InitializeComponent();

            if(sharedInstance==null)
            {
                sharedInstance = this;
            }
            panelREVIRE.Controls.Add(Inicio);


     

            Vector3[] seq1 = new Vector3[]
       {
            new Vector3(0, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(2, 2, 0),
            new Vector3(3, 3, 0)
       };

            Vector3[] seq2 = new Vector3[]
            {
            new Vector3(0, 0, 0),
            new Vector3(1.1f, 1, 0),           
            new Vector3(2.2f, 2, 0),
            new Vector3(3, 3, 0)
            };

            var result = DTW.Calculate(seq1, seq2, windowSize: 1);

            Console.WriteLine($"Distancia DTW: {result.Distance:F4}");
            Console.WriteLine("Camino de alineación:");
            foreach (var step in result.Path)
            {
                Console.WriteLine($"seq1[{step[0]}] <-> seq2[{step[1]}]");
            }

        }

        public Panel GetPanelREVIRE()
        {
            return panelREVIRE;
        }
    }
}
