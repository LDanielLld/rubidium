using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace REVIREPanels.Componentes
{
    public partial class PanelForceOrVel : UserControl
    {
        public static PanelForceOrVel sharedInstance = null;

        float vright, vleft, vback, vforward;

        public PanelForceOrVel()
        {
            InitializeComponent();

            if (sharedInstance == null)
            {
                sharedInstance = this;
            }

            vright = 0;
            vleft = 0;
            vback = 0;
            vforward = 0;
        }

        public float[] GetValues()
        {
            float[] values = new float[4];
            values[0] = float.Parse(lblLeft.Text);// vleft;
            values[1] = float.Parse(lblRight.Text);//vright;
            values[2] = float.Parse(lblBack.Text);//vback;
            values[3] = float.Parse(lblForward.Text);//vforward;
            return values;
        }

        private void trackBarBack_Scroll(object sender, EventArgs e)
        {
            int value = SetDataTrackBar(trackBarBack, lblBack);

            //Modificar el eje contrario            
            trackBarForward.Value = trackBarForward.Maximum;
            lblForward.Text = "0";
        }

        private void trackBarLeft_Scroll(object sender, EventArgs e)
        {
            int value = SetDataTrackBar(trackBarLeft, lblLeft);

            //Modificar el eje contrario
            ResetDataTrackBar(trackBarRight, lblRight);
        }

        private void trackBarRight_Scroll(object sender, EventArgs e)
        {
            int value = SetDataTrackBar(trackBarRight, lblRight);

            //Modificar el eje contrario
            ResetDataTrackBar(trackBarLeft, lblLeft);
        }

        private void trackBarForward_Scroll(object sender, EventArgs e)
        {
            int valuecurrent = trackBarForward.Value;
            int value = trackBarForward.Maximum - valuecurrent;
            lblForward.Text = value.ToString();
            //int value = SetDataTrackBar(trackBarForward, lblForward);

            //Modificar el eje contrario
            ResetDataTrackBar(trackBarBack, lblBack);

        }

        private int SetDataTrackBar(TrackBar trackBar, Label label)
        {
            int value = trackBar.Value;
            label.Text = value.ToString();
            return value;
        }

        private void ResetDataTrackBar(TrackBar trackBar, Label label)
        {
            trackBar.Value = 0;
            label.Text = "0";
        }


        //---------------------Botones de subir o bajar valores---------------------------------//
        //--------------------------------------------------------------------------------------//
        private void btnAddForward_Click(object sender, EventArgs e)
        {
            if (trackBarForward.Value > trackBarForward.Minimum)
            {
                --trackBarForward.Value;


                int value = trackBarForward.Maximum - trackBarForward.Value;
                lblForward.Text = value.ToString();
                //int value = SetDataTrackBar(trackBarForward, lblForward);

                //Modificar el eje contrario
                ResetDataTrackBar(trackBarBack, lblBack);
            }

        }

        private void btnAddRight_Click(object sender, EventArgs e)
        {
            if (trackBarRight.Value < trackBarRight.Maximum)
            {
                ++trackBarRight.Value;
                int value = SetDataTrackBar(trackBarRight, lblRight);
            }

            //Modificar el eje contrario
            ResetDataTrackBar(trackBarLeft, lblLeft);
        }

        private void btnAddBack_Click(object sender, EventArgs e)
        {
            if (trackBarBack.Value < trackBarBack.Maximum)
            {
                ++trackBarBack.Value;
                int value = SetDataTrackBar(trackBarBack, lblBack);
            }

            //Modificar el eje contrario            
            trackBarForward.Value = trackBarForward.Maximum;
            lblForward.Text = "0";
        }

        private void btnAddLeft_Click(object sender, EventArgs e)
        {
            if (trackBarLeft.Value < trackBarLeft.Maximum)
            {
                ++trackBarLeft.Value;
                int value = SetDataTrackBar(trackBarLeft, lblLeft);
            }

            //Modificar el eje contrario
            ResetDataTrackBar(trackBarRight, lblRight);
        }

        private void btnSubsForward_Click(object sender, EventArgs e)
        {
            if (trackBarForward.Value < trackBarForward.Maximum)
            {
                ++trackBarForward.Value;


                int value = trackBarForward.Maximum - trackBarForward.Value;
                lblForward.Text = value.ToString();
                //int value = SetDataTrackBar(trackBarForward, lblForward);

                //Modificar el eje contrario
                ResetDataTrackBar(trackBarBack, lblBack);
            }

        }

        private void btnSubsRight_Click(object sender, EventArgs e)
        {
            if (trackBarRight.Value > trackBarRight.Minimum)
            {
                --trackBarRight.Value;
                int value = SetDataTrackBar(trackBarRight, lblRight);

                //Modificar el eje contrario
                ResetDataTrackBar(trackBarLeft, lblLeft);
            }
        }

        private void btnSubsBack_Click(object sender, EventArgs e)
        {
            if (trackBarBack.Value > trackBarBack.Minimum)
            {
                --trackBarBack.Value;
                int value = SetDataTrackBar(trackBarBack, lblBack);

                //Modificar el eje contrario            
                trackBarForward.Value = trackBarForward.Maximum;
                lblForward.Text = "0";
            }


        }

        private void btnSubsLeft_Click(object sender, EventArgs e)
        {
            if (trackBarLeft.Value > trackBarLeft.Minimum)
            {
                --trackBarLeft.Value;
                int value = SetDataTrackBar(trackBarLeft, lblLeft);

                //Modificar el eje contrario
                ResetDataTrackBar(trackBarRight, lblRight);
            }


        }
        //--------------------------------------------------------------------------------------//
        //--------------------------------------------------------------------------------------//
    }
}
