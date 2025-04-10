using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using REVIREPanels.Estadisticas.Graficas;
using System.Windows.Forms.DataVisualization.Charting;
using REVIREPanels.Modelos;
using REVIREPanels.Componentes;

namespace REVIREPanels.Estadisticas.Paneles
{
    public partial class PanelInfoPerform : UserControl
    {        
        private BarDataChart barchart; //Controlador de la grafica

        //Lista de actividades
        private List<Actividad> current_lista_actividades;
        private List<string> names;

        public PanelInfoPerform()
        {
            InitializeComponent();

            // Set Antialiasing mode
            chartData.AntiAliasing = AntiAliasingStyles.All;
            chartData.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
        }


        public void Setting(List<Actividad> list)
        {
            current_lista_actividades = list;

            names = Util.GetListNameTask(current_lista_actividades);
        }      
                
                    
             
        //*********************Visualizar datos de rendimiento************************//
        //****************************************************************************//
        #region Datos Grafica rendimiento
        private void cb_Rep_CheckedChanged(object sender, EventArgs e)
        {
            
            if (cb_Rep.Checked)
            {
                cb_Distance.Checked = false;
                cb_SuccesFails.Checked = false; //Quita seleccion anterior
                cb_Time.Checked = false;
                cb_Distance.Checked = false;

                //Recoger los datos de la sesion                    
                IEnumerable<double> values = Util.GetNumericInfoTasks(1, current_lista_actividades);

                //Configura grafica de datos para la visualizacion de posicion
                barchart = new BarDataChart(chartData, BarChartType.REPETITIONS);
                barchart.Update(values, names);

            }
            else
            {
                barchart.Reset();
            }
            
        }

        private void cb_SuccesFails_CheckedChanged(object sender, EventArgs e)
        {
            
            if (cb_SuccesFails.Checked)
            {
                cb_Distance.Checked = false;
                cb_Rep.Checked = false; //Quita seleccion anterior
                cb_Time.Checked = false;
                cb_Distance.Checked = false;

                //Recoger los datos de la sesion                    
                IEnumerable<double> success = Util.GetNumericInfoTasks(2, current_lista_actividades);
                IEnumerable<double> fails = Util.GetNumericInfoTasks(5, current_lista_actividades);

                //Configura grafica de datos para la visualizacion de posicion
                barchart = new BarDataChart(chartData, BarChartType.REP_SUCCES_FAIL);
                barchart.UpdateStackedBar(success, fails, names);                    
            }
            else
            {
                barchart.Reset();
            }
            
        }

        private void cb_Time_CheckedChanged(object sender, EventArgs e)
        {
           
            if (cb_Time.Checked)
            {
                //Primero quita seleccion anterior para que no se solape
                cb_Distance.Checked = false;
                cb_Rep.Checked = false; //Quita seleccion anterior
                cb_SuccesFails.Checked = false;
                cb_Distance.Checked = false;

                //Recoger los datos de la sesion                    
                IEnumerable<double> values = Util.GetNumericInfoTasks(4, current_lista_actividades);

                //Configura grafica de datos para la visualizacion del tiempo
                barchart = new BarDataChart(chartData, BarChartType.TIME_TOTAL);
                barchart.Update(values, names);
            }
            else
            {
                barchart.Reset();
            }
            
        }


        
        private void cb_Distance_CheckedChanged(object sender, EventArgs e)
        {
            
            if (cb_Distance.Checked)
            {
                //Primero quita seleccion anterior para que no se solape
                cb_Time.Checked = false;
                cb_Rep.Checked = false; //Quita seleccion anterior
                cb_SuccesFails.Checked = false;


                //Recoger los datos de la sesion                    
                IEnumerable<double> values = Util.GetNumericInfoTasks(3, current_lista_actividades);

                //Configura grafica de datos para la visualizacion de las distancias
                barchart = new BarDataChart(chartData, BarChartType.DISTANCE_TOTAL);
                barchart.Update(values, names);
            }
            else
            {
                barchart.Reset();
            }
            
        }
        #endregion

        
        //****************************************************************************//
        //****************************************************************************//


       
    }
}

