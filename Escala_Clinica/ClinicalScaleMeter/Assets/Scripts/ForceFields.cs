using System;
using System.Threading;

/// <summary>
/// Class to create force fields for the Rubidium robot
/// </summary>
public class ForceFields
{
    #region Class Fields

    ////////////////////////////////////////////////////////////////////////////
    // Variables for calculating the approximation vector to the trajectory
    ////////////////////////////////////////////////////////////////////////////

    
    private float[] trajecStart;
    /// <summary>
    /// Initial point of the trajectory
    /// </summary>
    public float[] StartPoint
    {
        get => trajecStart;
        protected set { }
    }

    /// End point of the trajectory
    private float[] trajecEnd;
    /// End point of the trajectory
    public float[] EndPoint
    {
        get => trajecEnd;
        protected set { }
    }

    /// Trajectory stored as the parameters A, B, C that define the line in which is contained
    private float[] trajectory;

    /// Unitary and orthonormal vector to the trajectory in one sense
    private float[] n1;

    /// Unitary and orthonormal vector to the trajectory in the opposite sense of n1
    private float[] n2;

    /// Unitary director vector of the trajectory
    private float[] utr;


    ////////////////////////////////////////////////////////////////////////////
    // Variables for calculating the assitance forces
    ////////////////////////////////////////////////////////////////////////////

    // Maximum applicable force by Rb
    private float Fmax;
    public float Fmax_robot
    {
        get => Fmax;
        protected set { }
    }

    // Configurable maximum applied force by Rb
    private float conf_Fmax;
    public float Fmax_task
    {
        get => Fmax;
        protected set { }
    }

    // Force to compensate friction
    private float F_fric;
    public float FrictionForce
    {
        get => F_fric;
        protected set { }
    }

    // Bowl force
    private float cGaussBowl;
    private float inv_cGaussBowl;
    private float bowlDistance;
    private float[] bowlDirection;
    private bool applyBowl;
    public bool BowlApplied
    {
        get => applyBowl;
        protected set { }
    }

    // Tunnel force
    private float cGaussTunnel;
    private float inv_cGaussTunnel;
    private float tunnelDistance;
    private float[] tunnelDirection;
    private bool applyTunnel;
    public bool TunnelApplied
    {
        get => applyTunnel;
        protected set { }
    }

    // Tunnel with extremes
    private bool applyExtremes;
    public bool ExtremesApplied
    {
        get => applyExtremes;
        protected set { }
    }
    private bool endReached;    // True if wall reaches the end of the trajectory

    // Constrictive tunnel force
    private float[] prevWallPos;    // Array to save previous position of the robot during the wall advance
    private float pushingSpeed;
    private bool applyPush;
    public bool ConstrTunnelApplied
    {
        get => applyPush;
        protected set { }
    }

    // Timing thread
    private Thread timingThread;
    private bool isTimingThreadOn;
    private int t_count_ms;
    private int t_start_ms;
    private int t_count_prev_ms;
    private int timer_freq_hz;
    private int timer_period_ms;

    #endregion



    #region Constructor & Destructor

    public ForceFields()
    {
        // Variables for calculating the approximation vector to the trajectory
        trajecStart = new float[2];
        trajecEnd = new float[2];
        trajectory = new float[3];
        n1 = new float[2];
        n2 = new float[2];
        utr = new float[2];

        // Variables for calculating the assitance 
        Fmax = 110.0f;
        conf_Fmax = Fmax;
        F_fric = 0.0f;

        // Bowl force
        cGaussBowl = 0.1f;
        inv_cGaussBowl = 1 / cGaussBowl;
        bowlDistance = 0.0f;
        bowlDirection = new float[2];
        applyBowl = false;

        // Tunnel force
        cGaussTunnel = 0.1f;
        inv_cGaussTunnel = 1 / cGaussTunnel;
        tunnelDistance = 0.0f;
        tunnelDirection = new float[2];
        applyTunnel = false;

        // Tunnel with extremes
        applyExtremes = false;
        endReached = false;

        // Constrictive tunnel force
        prevWallPos = new float[] { 0.0f, -0.5f };
        pushingSpeed = 0.0f;
        applyPush = false;

        // Timing thread
        isTimingThreadOn = false;
        t_count_ms = 0;
        t_start_ms = 0;
        t_count_prev_ms = 0;
        timer_freq_hz = 100;
        timer_period_ms = 1000 / timer_freq_hz;
    }

    ~ForceFields()
    {
        CloseTimingThread();
    }

    #endregion



    #region Assistance

    public void UpdateTrajectory(float xi, float yi, float xf, float yf)
    {
        trajecStart[0] = xi;
        trajecStart[1] = yi;
        trajecEnd[0] = xf;
        trajecEnd[1] = yf;
        
        endReached = false;

        float zero = 0.0000001f;

        // Director vector
        float ux = xf - xi;
        float uy = yf - yi;

        // Normalize director vector
        float norm = (float)Math.Sqrt(Math.Pow(ux, 2.0f) + Math.Pow(uy, 2.0f));
        utr[0] = ux / norm;
        utr[1] = uy / norm;

        if (Math.Abs(utr[0]) > zero || Math.Abs(utr[1]) > zero)
        {
            // Trajectory will be calculated in this format: Ax + By + C = 0
            // The trajectory has this director vector and contains the point (xi, yi)
            trajectory[0] = utr[1];
            trajectory[1] = -1.0f * utr[0];
            trajectory[2] = -utr[1] * xi + utr[0] * yi;
        }

        OrtNormVectorToTrajec(trajectory);
    }

    public float[] ComputeForces(float px, float py, float[] bowlCenter)
    {
        float[] F_bowl = { 0.0f, 0.0f };
        float[] F_tunnel = { 0.0f, 0.0f };
        float[] F_push = { 0.0f, 0.0f };
        float[] F_extr = { 0.0f, 0.0f };
        float[] F_out = { 0.0f, 0.0f };

        if (applyBowl)
            F_bowl = BowlForce(bowlCenter[0], bowlCenter[1], px, py);

        if (applyTunnel)
            F_tunnel = TunnelForce(px, py);

        if (applyPush)
            F_push = ConstrictiveTunnelForce(px, py);

        if (applyExtremes)
            F_extr = TunnelWithExtremes(px, py);


        F_out[0] = F_bowl[0] + F_tunnel[0] + F_push[0] + F_extr[0];
        F_out[1] = F_bowl[1] + F_tunnel[1] + F_push[1] + F_extr[1];

        return F_out;

    }

    #endregion



    #region Configuration Methods

    public bool SetFmaxTask(float new_Fmax)
    {
        bool Fmax_changed = false;

        if (new_Fmax <= Fmax)
        {
            conf_Fmax = new_Fmax;
            Fmax_changed = true;
        }

        return Fmax_changed;
    }

    public bool SetFfric(float new_Ffric)
    {
        bool Ffric_changed = false;
        if (new_Ffric <= conf_Fmax)
        {
            F_fric = new_Ffric;
            Ffric_changed = true;
        }

        return Ffric_changed;
    }

    #endregion



    #region Bowl Force

    private float[] BowlForce(float x_center, float y_center, float px, float py)
    {
        float[] F_bowl = new float[2];

        DistanceToBowlCenter(x_center, y_center, px, py);

        if (bowlDistance != 0.0f)
        {
            // Direction of the force vector to approximate towards the center
            CalcBowlForceDirection(x_center, y_center, px, py);

            // Magnitude of gaussian force
            float Fg = conf_Fmax * (float)(1 - Math.Exp(-0.5 * Math.Pow(bowlDistance * inv_cGaussBowl, 2))) + F_fric;

            // Force vector to apply
            F_bowl[0] = Fg * bowlDirection[0];
            F_bowl[1] = Fg * bowlDirection[1];
        }
        else
        {
            F_bowl[0] = 0.0f;
            F_bowl[1] = 0.0f;
        }

        return F_bowl;
    }

    private void CalcBowlForceDirection(float x_center, float y_center, float px, float py)
    {
        bowlDirection[0] = (x_center - px) / bowlDistance;
        bowlDirection[1] = (y_center - py) / bowlDistance;
    }

    private void DistanceToBowlCenter(float xi, float yi, float xf, float yf)
    {
        float dx = xf - xi;
        float dy = yf - yi;

        bowlDistance = (float)Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
    }

    public void SetBowlForce()
    {
        RemoveAllForces();
        applyBowl = true;
    }

    public void RemoveBowlForce()
    {
        applyBowl = false;
    }

    public void ConfigBowlForce(float cGauss)
    {
        cGaussBowl = cGauss;
        inv_cGaussBowl = 1 / cGauss;
    }

    #endregion



    #region Tunnel Force

    private float[] TunnelForce(float px, float py)
    {
        float[] F_tunnel = new float[2];

        // Distance to the trajectory
        DistanceToTunnel(px, py);

        if (tunnelDistance != 0.0f)
        {
            // Direction of the force vector to approximate towards the trajectory
            CalcTunnelForceDirection(px, py);

            // Magnitude of gaussian force
            float Fg = conf_Fmax * (float)(1.0 - Math.Exp(-0.5 * Math.Pow(tunnelDistance * inv_cGaussTunnel, 2.0))) + F_fric;

            // Force vector to apply        
            F_tunnel[0] = Fg * tunnelDirection[0];
            F_tunnel[1] = Fg * tunnelDirection[1];
        }
        else
        {
            F_tunnel[0] = 0.0f;
            F_tunnel[1] = 0.0f;
        }

        return F_tunnel;
    }

    private void CalcTunnelForceDirection(float px, float py)
    {
        float A = trajectory[0];
        float B = trajectory[1];
        tunnelDirection = new float[2] { 0.0f, 0.0f };

        if (A > 0 && B > 0)
        {
            if (tunnelDistance > 0)
            {
                tunnelDirection[0] = n2[0];
                tunnelDirection[1] = n2[1];
            }
            else if (tunnelDistance < 0)
            {
                tunnelDirection[0] = n1[0];
                tunnelDirection[1] = n1[1];
            }
        }
        else if (A > 0 && B == 0)
        {
            if (px > trajecEnd[0])
            {
                tunnelDirection[0] = n2[0];
                tunnelDirection[1] = n2[1];
            }
            else if (px < trajecEnd[0])
            {
                tunnelDirection[0] = n1[0];
                tunnelDirection[1] = n1[1];
            }
        }
        else if (A > 0 && B < 0)
        {
            if (tunnelDistance > 0)
            {
                tunnelDirection[0] = n2[0];
                tunnelDirection[1] = n2[1];
            }
            else if (tunnelDistance < 0)
            {
                tunnelDirection[0] = n1[0];
                tunnelDirection[1] = n1[1];
            }
        }
        else if (A == 0 && B > 0)
        {
            if (py > trajecEnd[1])
            {
                tunnelDirection[0] = n2[0];
                tunnelDirection[1] = n2[1];
            }
            else if (py < trajecEnd[1])
            {
                tunnelDirection[0] = n1[0];
                tunnelDirection[1] = n1[1];
            }
        }
        else if (A == 0 && B < 0)
        {
            if (py > trajecEnd[1])
            {
                tunnelDirection[0] = n1[0];
                tunnelDirection[1] = n1[1];
            }
            else if (py < trajecEnd[1])
            {
                tunnelDirection[0] = n2[0];
                tunnelDirection[1] = n2[1];
            }
        }
        else if (A < 0 && B > 0)
        {
            if (tunnelDistance > 0)
            {
                tunnelDirection[0] = n2[0];
                tunnelDirection[1] = n2[1];
            }
            else if (tunnelDistance < 0)
            {
                tunnelDirection[0] = n1[0];
                tunnelDirection[1] = n1[1];
            }
        }
        else if (A < 0 && B == 0)
        {
            if (px > trajecEnd[0])
            {
                tunnelDirection[0] = n1[0];
                tunnelDirection[1] = n1[1];
            }
            else if (px < trajecEnd[0])
            {
                tunnelDirection[0] = n2[0];
                tunnelDirection[1] = n2[1];
            }
        }
        else if (A < 0 && B < 0)
        {
            if (tunnelDistance > 0)
            {
                tunnelDirection[0] = n2[0];
                tunnelDirection[1] = n2[1];
            }
            else if (tunnelDistance < 0)
            {
                tunnelDirection[0] = n1[0];
                tunnelDirection[1] = n1[1];
            }
        }
    }

    private void DistanceToTunnel(float px, float py)
    {
        float A = trajectory[0];
        float B = trajectory[1];
        float C = trajectory[2];

        tunnelDistance = (float)(A * px + B * py + C) / (float)Math.Sqrt(Math.Pow(A, 2) + Math.Pow(B, 2));
    }

    public void SetTunnelForce()
    {
        RemoveAllForces();
        applyTunnel = true;
    }

    public void RemoveTunnelForce()
    {
        applyTunnel = false;
    }

    public void ConfigTunnelForce(float cGauss)
    {
        cGaussTunnel = cGauss;
        inv_cGaussTunnel = 1 / cGauss;
    }

    #endregion



    #region Tunnel With Extremes

    private float[] TunnelWithExtremes(float px, float py)
    {
        float[] f = { 0.0f, 0.0f };

        // Create a line from (px, py) perpendicular to trajectory and find the intersection point
        float[] Q = ProjectPointOnTrajectory(px, py);

        // Check if Q is between the initial point and ending point of the trajectory
        if (utr[0] > 0)
        {
            if (Q[0] < trajecStart[0])
                f = BowlForce(trajecStart[0], trajecStart[1], px, py);
            else if (Q[0] > trajecEnd[0])
                f = BowlForce(trajecEnd[0], trajecEnd[1], px, py);
            else
                f = TunnelForce(px, py);
        }
        else if (utr[0] < 0)
        {
            if (Q[0] < trajecEnd[0])
                f = BowlForce(trajecEnd[0], trajecEnd[1], px, py);
            else if (Q[0] > trajecStart[0])
                f = BowlForce(trajecStart[0], trajecStart[1], px, py);
            else
                f = TunnelForce(px, py);
        }
        else
        {
            if (utr[1] > 0)
            {
                if (Q[1] < trajecStart[1])
                    f = BowlForce(trajecStart[0], trajecStart[1], px, py);
                else if (Q[1] > trajecEnd[1])
                    f = BowlForce(trajecEnd[0], trajecEnd[1], px, py);
                else
                    f = TunnelForce(px, py);
            }
            else if (utr[1] < 0)
            {
                if (Q[1] < trajecEnd[1])
                    f = BowlForce(trajecEnd[0], trajecEnd[1], px, py);
                else if (Q[1] > trajecStart[1])
                    f = BowlForce(trajecStart[0], trajecStart[1], px, py);
                else
                    f = TunnelForce(px, py);
            }
        }
        return f;
    }

    private float[] ProjectPointOnTrajectory(float px, float py)
    {
        // This function calculates point Q, which is the projection of the point P (user) over the trajectory t

        // Parameters of the line "r" which is perpendicular to trajectory "t" and contains the point P
        float Ar = n1[1];
        float Br = -n1[0];
        float Cr = py * n1[0] - px * n1[1];

        // Parameters of the trajectory "t"
        float At = trajectory[0];
        float Bt = trajectory[1];
        float Ct = trajectory[2];

        float[] Q = new float[2];
        Q[1] = (At * Cr - Ar * Ct) / (Ar * Bt - At * Br);
        Q[0] = -(Br * Q[1] + Cr) / Ar;

        return Q;
    }

    public void SetTunnelWithExtremes()
    {
        RemoveAllForces();
        applyExtremes = true;
    }

    public void RemoveTunnelWithExtremes()
    {
        applyExtremes = false;
    }

    public void ConfigTunnelWithExtremes(float cGauss)
    {
        cGaussBowl = cGauss;
        inv_cGaussBowl = 1 / cGauss;

        cGaussTunnel = cGauss;
        inv_cGaussTunnel = 1 / cGauss;
    }

    #endregion



    #region Constrictive tunnel force

    private float[] ConstrictiveTunnelForce(float px, float py)
    {
        float[] f = new float[2];
        float xWall, yWall;         // Position of the pushing wall

        if (t_count_ms < 0.0)
        {
            f = TunnelWithExtremes(px, py);

            prevWallPos[0] = trajecStart[0];
            prevWallPos[1] = trajecStart[1];
        }
        else
        {

            // Position of the force wall
            xWall = prevWallPos[0] + pushingSpeed * (float)(t_count_ms - t_count_prev_ms) * 0.001f * utr[0];
            yWall = prevWallPos[1] + pushingSpeed * (float)(t_count_ms - t_count_prev_ms) * 0.001f * utr[1];

            // Create a line from (px, py) perpendicular to trajectory and find the intersection point
            //float[] Q = ProjectPointOnTrajectory(xWall, yWall);
            float[] Q = { xWall, yWall };

            // Check if Q is between the starting point and ending point of the trajectory
            if (utr[0] > 0)
            {
                if (Q[0] > trajecEnd[0])
                    endReached = true;
            }
            else if (utr[0] < 0)
            {
                if (Q[0] < trajecEnd[0])
                    endReached = true;
            }
            else
            {
                if (utr[1] > 0)
                {
                    if (Q[1] > trajecEnd[1])
                        endReached = true;
                }
                else if (utr[1] < 0)
                {
                    if (Q[1] < trajecEnd[1])
                        endReached = true;
                }
            }

            if (endReached)
            {
                CloseTimingThread();

                xWall = trajecEnd[0];
                yWall = trajecEnd[1];

                f = BowlForce(trajecEnd[0], trajecEnd[1], px, py);
            }
            else
            {
                // Save wall position for the next iteration
                prevWallPos[0] = xWall;
                prevWallPos[1] = yWall;

                // Apply force field
                f = TunnelWithExtremes(px, py, xWall, yWall);
            }

        }

        t_count_prev_ms = t_count_ms;

        return f;
    }

    private float[] TunnelWithExtremes(float px, float py, float xWall, float yWall)
    {
        float[] f = { 0.0f, 0.0f };

        // Create a line from (px, py) perpendicular to trajectory and find the intersection point
        float[] Q = ProjectPointOnTrajectory(px, py);

        // Check if Q is between the initial point and ending point of the trajectory
        if (utr[0] > 0)
        {
            if (Q[0] < xWall)
                f = BowlForce(xWall, yWall, px, py);
            else if (Q[0] > trajecEnd[0])
                f = BowlForce(trajecEnd[0], trajecEnd[1], px, py);
            else
                f = TunnelForce(px, py);
        }
        else if (utr[0] < 0)
        {
            if (Q[0] < trajecEnd[0])
                f = BowlForce(trajecEnd[0], trajecEnd[1], px, py);
            else if (Q[0] > xWall)
                f = BowlForce(xWall, yWall, px, py);
            else
                f = TunnelForce(px, py);
        }
        else
        {
            if (utr[1] > 0)
            {
                if (Q[1] < yWall)
                    f = BowlForce(xWall, yWall, px, py);
                else if (Q[1] > trajecEnd[1])
                    f = BowlForce(trajecEnd[0], trajecEnd[1], px, py);
                else
                    f = TunnelForce(px, py);
            }
            else if (utr[1] < 0)
            {
                if (Q[1] < trajecEnd[1])
                    f = BowlForce(trajecEnd[0], trajecEnd[1], px, py);
                else if (Q[1] > yWall)
                    f = BowlForce(xWall, yWall, px, py);
                else
                    f = TunnelForce(px, py);
            }
        }
        return f;
    }

    public string SetConstrictiveTunnel()
    {
        RemoveAllForces();

        applyPush = true;

        string state;

        if (isTimingThreadOn)
            state = ResetTimer();
        else
            state = InitTimingThread();

        return state;
    }

    public void RemoveConstrictiveTunnel()
    {
        CloseTimingThread();
        applyPush = false;
    }

    public void ConfigConstrictiveTunnel(int timeDelay_ms, float speed)
    {
        if (timeDelay_ms <= 0)
            t_start_ms = 0;
        else
            t_start_ms = timeDelay_ms;

        pushingSpeed = Math.Abs(speed);
    }

    #endregion



    #region Auxiliar methods

    private float CalcGaussForce(float dist, float c, float F_fric)
    {
        // Gaussian force field calculated according to Eq.16 in paper "Metodos de control
        // basados en campos potenciales y de fuerza para robotica de rehabilitacion"
        return conf_Fmax * (float)(1 - Math.Exp(-0.5 * Math.Pow(dist / c, 2))) + F_fric;
    }

    public float CalcGaussC(float dist, float force, float F_fric)
    {
        // Result of solving the gaussian force expression for variable "c" of the denominator
        return dist / (float)(Math.Sqrt(2 * Math.Log(conf_Fmax / (conf_Fmax - force + F_fric))));
    }

    public float CalcGaussDist(float c, float force, float F_fric)
    {
        // Distance at which the gaussian force has the entered value, for a given coefficient "c"
        return c * (float)(Math.Sqrt(2 * Math.Log(conf_Fmax / (conf_Fmax - force + F_fric))));
    }

    private void RemoveAllForces()
    {
        RemoveBowlForce();
        RemoveTunnelForce();
        RemoveTunnelWithExtremes();
        RemoveConstrictiveTunnel();
    }

    private void OrtNormVectorToTrajec(float[] trajectory)
    {
        float[] utr = new float[2];
        utr[0] = -1 * trajectory[1];
        utr[1] = trajectory[0];

        // Vector n1
        n1[0] = utr[1];
        n1[1] = -1 * utr[0];
        // Vector n2
        n2[0] = -1 * n1[0];
        n2[1] = -1 * n1[1];
    }

    #endregion



    #region Timing Thread

    private void Timer()
    {
        while (isTimingThreadOn)
        {
            t_count_ms++;
            Thread.Sleep(1);
        }
        
    }

    private string InitTimingThread()
    {
        string state = "";

        try
        {
            timingThread = new Thread(new ThreadStart(Timer));
            timingThread.IsBackground = true;

            isTimingThreadOn = true;
            t_count_ms = -t_start_ms;

            timingThread.Start();
            state = "Timer thread initialized!!";
        }
        catch (Exception e)
        {
            state = e.Message.ToString();
            isTimingThreadOn = false;
        }

        return state;
    }

    private void CloseTimingThread()
    {
        if (timingThread != null)
        {
            isTimingThreadOn = false;
            timingThread.Abort();
            timingThread = null;
        }

    }

    private string ResetTimer()
    {
        string state = "";
        if (timingThread.IsAlive && isTimingThreadOn)
        {
            try
            {
                CloseTimingThread();
                InitTimingThread();
                state = "Timer succesfully reseted";
            }
            catch (Exception e)
            {
                state = e.Message;
            }
        }

        return state;
    }

    #endregion

}
