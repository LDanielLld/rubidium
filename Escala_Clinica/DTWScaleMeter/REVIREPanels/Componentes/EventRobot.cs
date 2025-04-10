using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace REVIREPanels.Componentes
{
    //Registra todos los movimientos realizados en un trial, 
    //ir a un objetivo desde otro punto, contando el tiempo de reposo
    public class TrialPacket
    {
        public uint sequence; //Numero de trial
        public double[] timeTarget;
        public int[] idTarget; //Objetivos (inicio y final)
        public byte[] typeTarget;

        public int totalBytes = 0;

        public TrialPacket()
        {
            //Numero de trial
            sequence = 0;
            totalBytes += sizeof(uint);

            //Tiempo inicio descanso, marca objetivo y final del trial
            timeTarget = new double[3] { 0, 0, 0 };
            totalBytes += sizeof(double) * 3;

            //Objetivos (inicio y final)
            idTarget = new int[2] { 0, 0 };
            totalBytes += sizeof(int) * 2;

            //Tipo de obejtivos
            typeTarget = new byte[2] { 0, 0 };
            totalBytes += sizeof(byte) * 2;
        }
    }

    public class EventRobot
    {
        public List<TrialPacket> Trials { get; set; }
        public List<Vector2> Target { get; set; }
        public List<Vector2> PosInit { get; set; }  
        
        public EventRobot()
        {
            Trials = new List<TrialPacket>();
            Target = new List<Vector2>();
            PosInit = new List<Vector2>();
        }
    }
}
