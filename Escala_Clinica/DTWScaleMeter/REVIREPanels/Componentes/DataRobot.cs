using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REVIREPanels
{
    public class DataRobot
    {
        //1 tiempo
        public double TimeStamp { get; set; }

        //12 datos
        public double WorkingMode { get; set; }
        public double Xpr { get; set; }
        public double Ypr { get; set; }
        public double Vxr { get; set; }
        public double Vyr { get; set; }
        public double Fxr { get; set; }
        public double Fyr { get; set; }
        public double EndeffAngle { get; set; }
        public double RobotActivated { get; set; }
        public double RightArmSat { get; set; }
        public double LeftArmSat { get; set; }
        public double Pulsed { get; set; }     

        //11 adicionales
        public double NivelAsistencia { get; set; }
        public double Fuerza { get; set; }
        public double TiempoMax { get; set; }
        public double Cx { get; set; }
        public double Cy { get; set; }
        public double GameState { get; set; }
        public double TaskState { get; set; }
        public double Xpr0 { get; set; } //Origen
        public double Ypr0 { get; set; }
        public double XprF { get; set; } //Destino
        public double YprF { get; set; }

        //7 valores de fuerzas
        public double Fx_ff { get; set; }
        public double Fy_ff { get; set; }
        public double Ff_origen_x { get; set; }
        public double Ff_origen_y { get; set; }
        public double Ff_destino_x { get; set; }
        public double Ff_destino_y { get; set; }
        public double CGauss { get; set; }        
    }    
}
