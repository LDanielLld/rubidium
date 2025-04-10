using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace REVIREPanels.Componentes
{
    enum EndpointStyle
    {
        ArrowHead,
        Fletching,
        Circle
    };

    enum BorderType
    {
        None,
        BorderRounded,
        BorderFixed,
        Border3D
    };

    public partial class StepInterface : UserControl
    {        
        //Atributos generales
        private readonly int stepsMax = 4;
        private List<string> listSteps = new List<string>() { "Paso 1", "Paso 2", "Paso 3", "Paso 4" };
        private List<string> listVideos = new List<string>() { "1_paso.mp4", "2_paso.mp4",
            "3_paso.mp4", "4_paso.mp4" };


        //Tamaños en porcentajes de pantalla
        private readonly int steps_perce_top = 15; //distancia vertical donde empiezan los pasos - porcentaje
        private readonly int steps_perce_botton = 50;

        private readonly int width_perce = 15; //Tamaño anchura de la parte que muestra el paso actual        
        private readonly float width_margin_video = 2.5f; //Margen del video

        //Tamaños en pixeles
        private int stepsMarginTop;
        private int stepsMarginBottom;

        private int stepNamePos;
        private int size_step;
        private int stepY_min;
        private int stepY_max;
        private int currentStep;

    


       

       

        public StepInterface()
        {
            InitializeComponent();

            tableLayoutIn.ColumnStyles[0].Width = width_perce + width_margin_video;
            tableLayoutIn.ColumnStyles[1].Width = 100 - (width_perce + width_margin_video) - width_margin_video;
            tableLayoutIn.ColumnStyles[2].Width = width_margin_video;

            //Elimiar parpadeo del refresco de los componentes visuales
            this.DoubleBuffered = true;
            enableDoubleBuff(tableLayoutIn);
        }

        public static void enableDoubleBuff(System.Windows.Forms.Control cont)
        {
            System.Reflection.PropertyInfo DemoProp = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            DemoProp.SetValue(cont, true, null);
        }

        private void StepInterface_Paint(object sender, PaintEventArgs e)
        {        
            //Suavizar la interfaz           
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            //Dibujar panel compuesto
            DrawRoundedRectangle(e.Graphics, new Pen(Brushes.WhiteSmoke, 2), e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width - 1,
                  e.ClipRectangle.Height - 1, 10, 10, BorderType.BorderRounded);


            //String con los pasos actuales
            StringFormat format = new StringFormat(); //Texto gris
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Center;
            format.Trimming = StringTrimming.EllipsisCharacter;
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            for (int i = 0; i < listSteps.Count; i++)
            {

                int pos = CalculatePositionNameStep(i);
                Font currentFont;
                Brush currentBrush;
                if (pos == stepNamePos)
                {
                    currentBrush = new SolidBrush(Color.Black);
                    currentFont = new Font(label1.Font.FontFamily, label1.Font.Size, FontStyle.Bold);
                }
                else
                {
                    currentBrush = new SolidBrush(SystemColors.ActiveBorder);
                    currentFont = label1.Font;
                }

                e.Graphics.DrawString(listSteps[i], currentFont, currentBrush, 25, pos, format);

            }
        }


        private void DrawRoundedRectangle(Graphics g, Pen p, int x, int y, int w, int h, int rx, int ry, BorderType type)
        {
            if (type == BorderType.BorderRounded)
            {
                //Panel redondeado
                GraphicsPath path = new GraphicsPath();

                path.AddLine(x + size_step, y + ry, x + size_step, y + stepY_min - ry);
                path.AddArc(x + size_step - rx, y + stepY_min - ry, rx, ry, 0, 91);
                path.AddLine(x + size_step - rx, y + stepY_min, x + rx, y + stepY_min);
                path.AddArc(x + 1, y + stepY_min, 2 * rx, 2 * ry, 270, -90);
                path.AddLine(x + 1, y + stepY_min + ry, x + 1, y + stepY_max - ry);
                path.AddArc(x + 1, y + stepY_max - 2 * ry, 2 * rx, 2 * ry, 180, -90);
                path.AddLine(x + rx, y + stepY_max, x + size_step - rx, y + stepY_max);
                path.AddArc(x + size_step - rx, y + stepY_max, rx, ry, 270, 90);
                path.AddLine(x + size_step, y + stepY_max + ry, x + size_step, y + h - ry);
                path.AddArc(x + size_step, y + h - 2 * ry, 2 * rx, 2 * ry, 180, -90);
                path.AddLine(x + size_step + rx, y + h, x + w - rx, y + h);
                path.AddArc(x + w - 2 * rx, y + h - 2 * ry, 2 * rx, 2 * ry, 90, -90);
                path.AddLine(x + w, y + h - ry, x + w, y + ry);
                path.AddArc(x + w - 2 * rx, y + 1, 2 * rx, 2 * ry, 0, -90);
                path.AddLine(x + w - rx, y + 1, x + size_step + rx, y + 1);
                path.AddArc(x + size_step, y + 1, 2 * rx, 2 * ry, 270, -90);

                path.CloseFigure();
                g.FillPath(Brushes.Gainsboro, path);
                g.DrawPath(p, path);
            }

            else if (type == BorderType.BorderFixed)
            {
                GraphicsPath path = new GraphicsPath();

                //Parte buena con esquinas 90º
                path.AddLine(x + size_step, y, x + size_step, y + stepY_min);

                path.AddLine(x + size_step, y + stepY_min, x, y + stepY_min);
                path.AddLine(x, y + stepY_min, x, y + stepY_max);
                path.AddLine(x, y + stepY_max, x + size_step, y + stepY_max);
                path.AddLine(x + size_step, y + stepY_max, x + size_step, y + h);
                path.AddLine(x + size_step, y + h, x + w, y + h);
                path.AddLine(x + w, y + h, x + w, y);

                path.CloseFigure();
                g.FillPath(Brushes.Gray, path);
                g.DrawPath(p, path);
                path.Dispose();
            }
            else if (type == BorderType.Border3D)
            {
                GraphicsPath pathUp = new GraphicsPath();
                GraphicsPath pathDown = new GraphicsPath();
                pathUp.AddArc(x, y, rx + rx, ry + ry, 180, 90);
                pathUp.AddLine(x + rx, y, x + w - rx, y);
                pathUp.AddArc(x + w - 2 * rx, y, 2 * rx, 2 * ry, 270, 90);
                pathUp.AddLine(x + w, y + ry, x + w, y + h - ry);
                // pathUp.CloseFigure();
                pathDown.AddArc(x + w - 2 * rx, y + h - 2 * ry, rx + rx, ry + ry, 0, 91);
                pathDown.AddLine(x + rx, y + h, x + w - rx, y + h);
                pathDown.AddArc(x, y + h - 2 * ry, 2 * rx, 2 * ry, 90, 91);
                pathDown.AddLine(x, y + ry, x, y + h - ry - 250);
                //GetPercentageHeight(int parts)

                // pathDown.CloseFigure();
                g.DrawPath(p, pathUp);
                Pen p2 = new Pen(Brushes.BlueViolet, 2);
                g.DrawPath(p2, pathDown);
            }
        }

        //****************************************************************************//

        public void CalculatePositionStep(int step)
        {
            currentStep = step;

            //Tamaños de margen
            stepsMarginTop = ClientRectangle.Height * steps_perce_top / 100;
            stepsMarginBottom = ClientRectangle.Height * steps_perce_botton / 100;

            int totalMargin = stepsMarginTop + stepsMarginBottom;
            int y_size = (ClientRectangle.Height - totalMargin) / stepsMax;
            stepY_min = y_size * step + stepsMarginTop;
            stepY_max = y_size * (step + 1) + stepsMarginTop;

            stepNamePos = stepY_max - (stepY_max - stepY_min) / 2;

            size_step = ClientRectangle.Width * width_perce / 100;


            label2.Text = "Paso " + (step + 1) + "/" + stepsMax;
            this.Invalidate();
        }

        private int CalculatePositionNameStep(int step)
        {
            //Tamaños de margen
            int stepsMarginTop = ClientRectangle.Height * steps_perce_top / 100;
            int stepsMarginBottom = ClientRectangle.Height * steps_perce_botton / 100;

            int totalMargin = stepsMarginTop + stepsMarginBottom;
            int y_size = (ClientRectangle.Height - totalMargin) / stepsMax;
            int stepY_min = y_size * step + stepsMarginTop;
            int stepY_max = y_size * (step + 1) + stepsMarginTop;

            int posname = stepY_max - (stepY_max - stepY_min) / 2;
            return posname;
        }

        public void LoadStep(int index)
        {
          
        }

    }   
}
