using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using REVIREPanels;

namespace REVIRE
{
    public partial class REVIRE : Form
    {        

        private PanelPrincipal principal;
        public PanelPrincipal Principal
        {
            get {
                if (principal == null)
                {
                    principal = new PanelPrincipal();
                    principal.Dock = DockStyle.Fill; //Para ajustar
                }                
                return principal;
            }
        }      


        public REVIRE()
        {
            InitializeComponent();
            panelMain.Controls.Add(Principal);

            //Elimiar parpadeo del refresco de los componentes visuales
            this.DoubleBuffered = true;
            enableDoubleBuff(panelMain);

            

            //Cambiar fuente del reloj
            //PrivateFontCollection pfc = new PrivateFontCollection();

            // Get the current directory.   
            /*string currentPathFont = Directory.GetCurrentDirectory() + @"\Resources\Font\ds_digital\";
            pfc.AddFontFile(currentPathFont + "DS-DIGI.ttf");
            mainTimer.Font = new Font(pfc.Families[0], 40, mainTimer.Font.Style);*/




        }

    

    public static void enableDoubleBuff(System.Windows.Forms.Control cont)
    {
        System.Reflection.PropertyInfo DemoProp = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        DemoProp.SetValue(cont, true, null);
    }

    private void timerGeneral_Tick(object sender, EventArgs e)
        {
            mainTimer.Text = DateTime.Now.ToShortTimeString();
        }


        private void linea_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle,
                SystemColors.ControlLightLight, 0, ButtonBorderStyle.Outset,
                SystemColors.ControlLightLight, 0, ButtonBorderStyle.Outset,
                SystemColors.ControlLightLight, 0, ButtonBorderStyle.Outset,
                SystemColors.ControlLightLight, 3, ButtonBorderStyle.Outset);
        }

        private void btnCloseAll_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
