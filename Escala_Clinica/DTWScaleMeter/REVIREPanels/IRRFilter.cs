using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REVIREPanels
{
    class IIRFilter
    {
        public float a0;
        public float a1;
        public float a2;
        public float b1;
        public float b2;

        public float x1;
        public float x2;
        public float y1;
        public float y2;

        // three parameters indicates a notch filter
        // equation obtained here http://dspguide.com/ch19/3.htm
        public IIRFilter(float samplingrate, float frequency, float Bandwidth)
        {
            float Pi = 3.141592f;
            float BW = Bandwidth / samplingrate;
            float f = frequency / samplingrate;
            float R = 1 - 3 * BW;
            float K = (1 - 2 * R * (float)Math.Cos(2 * Pi * f) + R * R) / (2 - 2 * (float)Math.Cos(2 * Pi * f));
            a0 = K;
            a1 = -2 * K * (float)Math.Cos(2 * Pi * f);
            a2 = K;
            b1 = 2 * R * (float)Math.Cos(2 * Pi * f);
            b2 = -R * R;

            x1 = x2 = y1 = y2 = 0;
        }

        // two parameters indicates a 2nd order Butterworth low-pass filter
        // equation obtained here: https://www.codeproject.com/Tips/1092012/A-Butterworth-Filter-in-Csharp
        public IIRFilter(float samplingrate, float frequency)
        {

            const float pi = 3.14159265358979f;
            float wc = (float)Math.Tan(frequency * pi / samplingrate);
            float k1 = 1.414213562f * wc;
            float k2 = wc * wc;
            a0 = k2 / (1 + k1 + k2);
            a1 = 2 * a0;
            a2 = a0;
            float k3 = a1 / k2;
            b1 = -2 * a0 + k3;
            b2 = 1 - (2 * a0) - k3;

            x1 = x2 = y1 = y2 = 0;
        }



        // for Butterworth high-pass filters or other IIR filters, you need to insert parameters manually.
        // Easy way to find these parameters is using R's "signal" package butter function, and convert parameters like this: 
        // a0 = $b[0]; a1 = $b[1]; a2 = $b[2]; b1 = -$a[1]; b2 = -$a[2]
        public IIRFilter(float a0in, float a1in, float a2in, float b1in, float b2in)
        {
            a0 = a0in;
            a1 = a1in;
            a2 = a2in;
            b1 = b1in;
            b2 = b2in;

            x1 = x2 = y1 = y2 = 0;
        }



        public double[] Butterworth(double[] indata, double deltaTimeinsec, double CutOff)
        {
            if (indata == null) return null;
            if (CutOff == 0) return indata;

            double Samplingrate = 1 / deltaTimeinsec;
            long dF2 = indata.Length - 1;        // The data range is set with dF2
            double[] Dat2 = new double[dF2 + 4]; // Array with 4 extra points front and back
            double[] data = indata; // Ptr., changes passed data

            // Copy indata to Dat2
            for (long r = 0; r < dF2; r++)
            {
                Dat2[2 + r] = indata[r];
            }
            Dat2[1] = Dat2[0] = indata[0];
            Dat2[dF2 + 3] = Dat2[dF2 + 2] = indata[dF2];

            const double pi = 3.14159265358979;
            double wc = Math.Tan(CutOff * pi / Samplingrate);
            double k1 = 1.414213562 * wc; // Sqrt(2) * wc
            double k2 = wc * wc;
            double a = k2 / (1 + k1 + k2);
            double b = 2 * a;
            double c = a;
            double k3 = b / k2;
            double d = -2 * a + k3;
            double e = 1 - (2 * a) - k3;

            // RECURSIVE TRIGGERS - ENABLE filter is performed (first, last points constant)
            double[] DatYt = new double[dF2 + 4];
            DatYt[1] = DatYt[0] = indata[0];
            for (long s = 2; s < dF2 + 2; s++)
            {
                DatYt[s] = a * Dat2[s] + b * Dat2[s - 1] + c * Dat2[s - 2]
                           + d * DatYt[s - 1] + e * DatYt[s - 2];
            }
            DatYt[dF2 + 3] = DatYt[dF2 + 2] = DatYt[dF2 + 1];

            // FORWARD filter
            double[] DatZt = new double[dF2 + 2];
            DatZt[dF2] = DatYt[dF2 + 2];
            DatZt[dF2 + 1] = DatYt[dF2 + 3];
            for (long t = -dF2 + 1; t <= 0; t++)
            {
                DatZt[-t] = a * DatYt[-t + 2] + b * DatYt[-t + 3] + c * DatYt[-t + 4]
                            + d * DatZt[-t + 1] + e * DatZt[-t + 2];
            }

            // Calculated points copied for return
            for (long p = 0; p < dF2; p++)
            {
                data[p] = DatZt[p];
            }

            return data;
        }
    }
}
