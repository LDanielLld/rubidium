using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace COM_OnRobot_HEX_H
{
    public class OnRobotHEX_H
    {
        #region Class Fields

        ///////////////////////////////
        // UDP parameters
        ///////////////////////////////
        private UdpClient sensorUdp;
        private IPEndPoint sensorEp;

        private int sensorUdpPort;
        public int SensorUdpPort
        {
            get => sensorUdpPort;
            protected set { }
        }

        private IPAddress ip;
        public string IP
        {
            get => ip.ToString();
            set => ip = IPAddress.Parse(value);
        }

        private bool isConnected;
        public bool IsConnected
        {
            get => isConnected;
            protected set { }
        }

        ///////////////////////////////
        // Parallel threads
        ///////////////////////////////
        private Thread receiveThread;
        private bool isRecvThread;
        public bool IsRecvThread
        {
            get => isRecvThread;
            protected set { }
        }

        ///////////////////////////////
        // Sensor data
        ///////////////////////////////
        private byte[] rawData;
        private byte[] hss;
        private byte[] fts;
        private byte[] sta;
        private byte[] fx, fy, fz;
        private byte[] tx, ty, tz;
        
        private float[] force;
        public float[] Force
        {
            get => force;
            protected set { }
        }
        
        private float[] torque;
        public float[] Torque
        {
            get => torque;
            protected set { }
        }
        
        private float[] info;
        public float[] Info
        {
            get => info;
            protected set { }
        }

        ///////////////////////////////
        // Sensor parameters
        ///////////////////////////////
        private int sampleFreq;
        private float samplePeriod;
        private float filterFreq;
        public float FilterFreq
        {
            get => filterFreq;
            protected set { }
        }
        private bool isOffsetCorrected;
        public bool IsOffsetCorrected
        {
            get => isOffsetCorrected;
            protected set { }
        }

        private bool isSensorStarted;
        public bool IsSensorStarted
        {
            get => isSensorStarted;
            protected set { }
        }

        ///////////////////////////////
        // Commands to control sensor
        ///////////////////////////////
        private UInt16 reqHeader;
        private UInt16 reqCommand;
        private UInt32 reqData;

        enum ReqCommandType : UInt16
        {
            Stop = 0x0000,
            Start = 0x0002,
            SetBias = 0x0042,
            SetFilterFreq = 0x0081,
            SetReadSpeed = 0x0082
        }

        #endregion

        #region Constructor & Destructor

        public OnRobotHEX_H()
        {
            // UDP parameters
            sensorUdpPort = 49152;
            ip = IPAddress.Parse("192.168.1.1");
            isConnected = false;
;
            // Parallel threads
            isRecvThread = false;

            // Sensor data
            hss = new byte[4];
            fts = new byte[4];
            sta = new byte[4];
            fx = new byte[4];
            fy = new byte[4];
            fz = new byte[4];
            tx = new byte[4];
            ty = new byte[4];
            tz = new byte[4];
            force = new float[3];
            torque = new float[3];
            info = new float[3];

            // Sensor parameters
            sampleFreq = 100;
            samplePeriod = 1 / sampleFreq;
            filterFreq = 15.0f;
            isOffsetCorrected = false;
            isSensorStarted = false;

            // Commands to control sensor
            reqHeader = 0x1234;
            reqCommand = 0x0000;
            reqData = 0x0000;
        }

        ~OnRobotHEX_H()
        {
            Disconnect();
        }

        #endregion

        #region Sensor Connection

        public string Connect()
        {
            string state;

            try
            {
                sensorEp = new IPEndPoint(ip, sensorUdpPort);
                sensorUdp = new UdpClient(sensorUdpPort);

                state = "UDP initialized!!";
                isConnected = true;
            }
            catch (Exception e)
            {
                state = e.Message.ToString();
                isConnected = false;
            }

            return state;
        }

        public string Disconnect()
        {
            string state = "";
            bool err = false;

            if (isConnected)
            {
                err = Stop();
                Thread.Sleep((int)samplePeriod * 10);
                isSensorStarted = false;
                isConnected = false;
            }

            if (sensorUdp != null)
            {
                sensorUdp.Close();
                sensorUdp = null;
            }

            if (isRecvThread)
            {
                receiveThread.Abort();
                receiveThread = null;
                isRecvThread = false;
            }

            if (err)
                state = "Communication closed! Warning: the sensor might be still sending info";
            else
                state = "Communication closed!";

            return state;
        }

        #endregion

        #region Data Reception

        public bool InitRecvThread()
        {
            string state = "";

            if (isConnected && !isRecvThread)
            {
                try
                {
                    receiveThread = new Thread(new ThreadStart(ReadUDP));
                    receiveThread.IsBackground = true;
                    receiveThread.Start();
                    state = "Receiving thread initialized!";
                    isRecvThread = true;
                }
                catch (Exception e)
                {
                    state = e.Message.ToString();
                    isRecvThread = false;
                }
            }

            return !isRecvThread;
        }

        private void ReadUDP()
        {
            try
            {
                while (isRecvThread)
                {                   
                    rawData = sensorUdp.Receive(ref sensorEp);
                    GetSensorData(rawData);
                    //Console.WriteLine(info[0].ToString() + " | " + info[1].ToString() + " | " + info[2].ToString() + " | " + force[0].ToString() + " | " + force[1].ToString() + " | " + force[2].ToString() + " | " + torque[0].ToString() + " | " + torque[1].ToString() + " | " + torque[2].ToString());
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
        }

        private bool GetSensorData(byte[] rawData)
        {
            bool err = true;

            if (rawData.Length == 36)
            {
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(rawData);

                    torque[2] = BitConverter.ToInt32(rawData, 0) * 0.00001f;
                    torque[1] = BitConverter.ToInt32(rawData, 4) * 0.00001f;
                    torque[0] = BitConverter.ToInt32(rawData, 8) * 0.00001f;
                    
                    force[2] = BitConverter.ToInt32(rawData, 12) * 0.0001f;
                    force[1] = BitConverter.ToInt32(rawData, 16) * 0.0001f;
                    force[0] = BitConverter.ToInt32(rawData, 20) * 0.0001f;
                    
                    info[2] = BitConverter.ToInt32(rawData, 24);
                    info[1] = BitConverter.ToInt32(rawData, 28);
                    info[0] = BitConverter.ToInt32(rawData, 32);
                }
                else
                {
                    info[0] = BitConverter.ToInt32(rawData, 0);
                    info[1] = BitConverter.ToInt32(rawData, 4);
                    info[2] = BitConverter.ToInt32(rawData, 8);
                    
                    force[0] = BitConverter.ToInt32(rawData, 12) * 0.0001f;
                    force[1] = BitConverter.ToInt32(rawData, 16) * 0.0001f;
                    force[2] = BitConverter.ToInt32(rawData, 20) * 0.0001f;
                    
                    torque[0] = BitConverter.ToInt32(rawData, 24) * 0.00001f;
                    torque[1] = BitConverter.ToInt32(rawData, 28) * 0.00001f;
                    torque[2] = BitConverter.ToInt32(rawData, 32) * 0.00001f;
                }

                err = false;
            }

            return err;
        }

        private void ExtractRawData(byte[] raw)
        {

            hss[0] = raw[0];
            hss[1] = raw[1];
            hss[2] = raw[2];
            hss[3] = raw[3];
            
            fts[0] = raw[4];
            fts[1] = raw[5];
            fts[2] = raw[6];
            fts[3] = raw[7];
            
            sta[0] = raw[8];
            sta[1] = raw[9];
            sta[2] = raw[10];
            sta[3] = raw[11];
            
            fx[0] = raw[12];
            fx[1] = raw[13];
            fx[2] = raw[14];
            fx[3] = raw[15];

            fy[0] = raw[16];
            fy[1] = raw[17];
            fy[2] = raw[18];
            fy[3] = raw[19];
            
            fz[0] = raw[20];
            fz[1] = raw[21];
            fz[2] = raw[22];
            fz[3] = raw[23];

            tx[0] = raw[24];
            tx[1] = raw[25];
            tx[2] = raw[26];
            tx[3] = raw[27];
            
            ty[0] = raw[28];
            ty[1] = raw[29];
            ty[2] = raw[30];
            ty[3] = raw[31];

            tz[0] = raw[32];
            tz[1] = raw[33];
            tz[2] = raw[34];
            tz[3] = raw[35];
        }

        #endregion

        #region Save Data

        public bool SaveData(BinaryWriter fileWriter)
        {
            bool err = false;

            try
            {
                fileWriter.Write(info[0]);
                fileWriter.Write(info[1]);
                fileWriter.Write(info[2]);
                fileWriter.Write(force[0]);
                fileWriter.Write(force[1]);
                fileWriter.Write(force[2]);
                fileWriter.Write(torque[0]);
                fileWriter.Write(torque[1]);
                fileWriter.Write(torque[2]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                err = true;
            }

            return err;
        }

        #endregion

        #region Sensor Control

        public bool Init()
        {
            bool err = true;

            if (IsConnected)
            {
                // No offset correction
                err = SetSoftwareBias(false);
                if (!err)
                {
                    // Cut-off frequency of the sensor internal filter = 15 Hz
                    err = SetInternalFiltering(4);
                }
                if (!err)
                {
                    // Sensor transmiting frequency = 100 Hz
                    if (SetReadingFreq(100) < 0)
                        err = true;
                }
                if (!err)
                {
                    // Init reception thread
                    err = InitRecvThread();
                }
                //if (!err)
                //{
                //    // Init sensor sending
                //    err = StartSensor(0);
                //}
            }
            return err;
        }

        public bool Start(int nSamples)
        {
            bool err = false;

            reqCommand = (UInt16)ReqCommandType.Start;
            reqData = (UInt16)nSamples;
            err = SendRequest(reqCommand, reqData);

            if (!err)
                isSensorStarted = true;

            return err;
        }

        public bool Stop()
        {
            bool err = false;

            reqCommand = (UInt16)ReqCommandType.Stop;
            reqData = 0x0000;
            err = SendRequest(reqCommand, reqData);

            if (!err)
                isSensorStarted = false;

            return err;
        }

        public bool SetSoftwareBias(bool set)
        {
            bool err = false;

            reqCommand = (UInt16)ReqCommandType.SetBias;
            if (set)
                reqData = (UInt16)255;
            else
                reqData = (UInt16)0;

            err = SendRequest(reqCommand, reqData);

            if (!err && set)
                isOffsetCorrected = true;
            else if (!err && !set)
                isOffsetCorrected = false;

            return err;
        }

        public bool SetInternalFiltering(int freq_index)
        {
            bool err = false;

            if (freq_index >= 0 && freq_index <= 6)
            {
                reqCommand = (UInt16)ReqCommandType.SetFilterFreq;
                reqData = (UInt16)freq_index;
                err = SendRequest(reqCommand, reqData);
                if (!err)
                {
                    switch (freq_index)
                    {
                        case 0:
                            filterFreq = 0.0f;
                            break;
                        case 1:
                            filterFreq = 500.0f;
                            break;
                        case 2:
                            filterFreq = 150.0f;
                            break;
                        case 3:
                            filterFreq = 50.0f;
                            break;
                        case 4:
                            filterFreq = 15.0f;
                            break;
                        case 5:
                            filterFreq = 5.0f;
                            break;
                        case 6:
                            filterFreq = 1.5f;
                            break;
                    }
                }
            }
            else
            {
                err = true;
            }

            return err;
        }

        public int SetReadingFreq(int freq)
        {
            bool err = false;
            int freq_out = -1;
            double newValue;

            if (freq >= 4 && freq <= 500)
            {
                newValue = 1000 / freq;
                if (newValue % 2 != 0)
                    Math.Round(newValue);

                freq_out = (int) (1000 / newValue);
                sampleFreq = freq_out;
                samplePeriod = 1 / freq_out;

                reqCommand = (UInt16)ReqCommandType.SetReadSpeed;
                reqData = (UInt16)newValue;
                err = SendRequest(reqCommand, reqData);

                if (err)
                    freq_out = -1;
            }

            return freq_out;
        }

        private bool SendRequest(UInt16 command, UInt32 data)
        {
            bool err = false;

            byte[] rawReqHeader = BitConverter.GetBytes(reqHeader);
            byte[] rawReqCommand = BitConverter.GetBytes(command);
            byte[] rawReqData = BitConverter.GetBytes(data);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(rawReqHeader);
                Array.Reverse(rawReqCommand);
                Array.Reverse(rawReqData);
            }

            try
            {
                byte[] request = new byte[rawReqHeader.Length + rawReqCommand.Length + rawReqData.Length];
                System.Buffer.BlockCopy(rawReqHeader, 0, request, 0, rawReqHeader.Length);
                System.Buffer.BlockCopy(rawReqCommand, 0, request, rawReqHeader.Length, rawReqCommand.Length);
                System.Buffer.BlockCopy(rawReqData, 0, request, rawReqHeader.Length + rawReqCommand.Length, rawReqData.Length);

                sensorUdp.Send(request, request.Length, sensorEp);
            }
            catch (Exception)
            {
                err = true;
            }

            return err;
        }

        #endregion

    }
}
