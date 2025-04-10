namespace REVIREPanels.Estadisticas.Paneles
{
    partial class PanelInfoPosition
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.chartData = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tableLayoutPanel13 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel21 = new System.Windows.Forms.TableLayoutPanel();
            this.cb_PosIdeal = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.ch_PosTargetZone = new System.Windows.Forms.CheckBox();
            this.cb_posTargets = new System.Windows.Forms.CheckBox();
            this.lblFocus = new System.Windows.Forms.Label();
            this.tableLayoutPanel14 = new System.Windows.Forms.TableLayoutPanel();
            this.cb_PosX = new System.Windows.Forms.CheckBox();
            this.cb_PosY = new System.Windows.Forms.CheckBox();
            this.cb_PosTrajectory = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.sampleSelector = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel15 = new System.Windows.Forms.TableLayoutPanel();
            this.cb_error_flag = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cb_TrialsSegmention = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.trialSelector = new System.Windows.Forms.NumericUpDown();
            this.btnApplyTrials = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.cb_initial_flag = new System.Windows.Forms.ComboBox();
            this.cb_final_flag = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartData)).BeginInit();
            this.tableLayoutPanel13.SuspendLayout();
            this.tableLayoutPanel21.SuspendLayout();
            this.tableLayoutPanel14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sampleSelector)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel15.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trialSelector)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 2;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.tableLayoutPanel10.Controls.Add(this.chartData, 1, 0);
            this.tableLayoutPanel10.Controls.Add(this.tableLayoutPanel13, 0, 0);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel10.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 1;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 683F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 683F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(1503, 632);
            this.tableLayoutPanel10.TabIndex = 10;
            // 
            // chartData
            // 
            this.chartData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(223)))), ((int)(((byte)(240)))));
            this.chartData.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            this.chartData.BackSecondaryColor = System.Drawing.Color.White;
            this.chartData.BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
            this.chartData.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chartData.BorderlineWidth = 2;
            this.chartData.BorderSkin.PageColor = System.Drawing.Color.Transparent;
            this.chartData.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
            chartArea1.AxisX.ScrollBar.ButtonColor = System.Drawing.Color.Silver;
            chartArea1.AxisX.ScrollBar.LineColor = System.Drawing.Color.Black;
            chartArea1.AxisY.ScrollBar.ButtonColor = System.Drawing.Color.Silver;
            chartArea1.AxisY.ScrollBar.LineColor = System.Drawing.Color.Black;
            chartArea1.BackColor = System.Drawing.Color.Transparent;
            chartArea1.IsSameFontSizeForAllAxes = true;
            chartArea1.Name = "ChartArea1";
            this.chartData.ChartAreas.Add(chartArea1);
            this.chartData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartData.Location = new System.Drawing.Point(526, 0);
            this.chartData.Margin = new System.Windows.Forms.Padding(0);
            this.chartData.Name = "chartData";
            this.chartData.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            this.chartData.Size = new System.Drawing.Size(977, 632);
            this.chartData.TabIndex = 7;
            this.chartData.AxisScrollBarClicked += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ScrollBarEventArgs>(this.chartData_AxisScrollBarClicked);
            // 
            // tableLayoutPanel13
            // 
            this.tableLayoutPanel13.ColumnCount = 2;
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel13.Controls.Add(this.tableLayoutPanel21, 0, 1);
            this.tableLayoutPanel13.Controls.Add(this.tableLayoutPanel14, 0, 0);
            this.tableLayoutPanel13.Controls.Add(this.groupBox3, 0, 2);
            this.tableLayoutPanel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel13.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel13.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel13.Name = "tableLayoutPanel13";
            this.tableLayoutPanel13.RowCount = 3;
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel13.Size = new System.Drawing.Size(526, 632);
            this.tableLayoutPanel13.TabIndex = 10;
            // 
            // tableLayoutPanel21
            // 
            this.tableLayoutPanel21.ColumnCount = 4;
            this.tableLayoutPanel13.SetColumnSpan(this.tableLayoutPanel21, 3);
            this.tableLayoutPanel21.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel21.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel21.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel21.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel21.Controls.Add(this.cb_PosIdeal, 0, 1);
            this.tableLayoutPanel21.Controls.Add(this.label10, 0, 0);
            this.tableLayoutPanel21.Controls.Add(this.ch_PosTargetZone, 0, 3);
            this.tableLayoutPanel21.Controls.Add(this.cb_posTargets, 0, 2);
            this.tableLayoutPanel21.Controls.Add(this.lblFocus, 3, 0);
            this.tableLayoutPanel21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel21.Location = new System.Drawing.Point(4, 163);
            this.tableLayoutPanel21.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel21.Name = "tableLayoutPanel21";
            this.tableLayoutPanel21.RowCount = 4;
            this.tableLayoutPanel21.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel21.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel21.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel21.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel21.Size = new System.Drawing.Size(518, 211);
            this.tableLayoutPanel21.TabIndex = 8;
            // 
            // cb_PosIdeal
            // 
            this.cb_PosIdeal.AutoSize = true;
            this.tableLayoutPanel21.SetColumnSpan(this.cb_PosIdeal, 2);
            this.cb_PosIdeal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_PosIdeal.Enabled = false;
            this.cb_PosIdeal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_PosIdeal.Location = new System.Drawing.Point(4, 57);
            this.cb_PosIdeal.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cb_PosIdeal.Name = "cb_PosIdeal";
            this.cb_PosIdeal.Size = new System.Drawing.Size(250, 42);
            this.cb_PosIdeal.TabIndex = 23;
            this.cb_PosIdeal.Text = "Trayectoria ideal";
            this.cb_PosIdeal.UseVisualStyleBackColor = true;
            this.cb_PosIdeal.CheckedChanged += new System.EventHandler(this.cb_PosIdeal_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.tableLayoutPanel21.SetColumnSpan(this.label10, 3);
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(4, 0);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(405, 52);
            this.label10.TabIndex = 30;
            this.label10.Text = "Características";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ch_PosTargetZone
            // 
            this.ch_PosTargetZone.AutoSize = true;
            this.tableLayoutPanel21.SetColumnSpan(this.ch_PosTargetZone, 2);
            this.ch_PosTargetZone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ch_PosTargetZone.Enabled = false;
            this.ch_PosTargetZone.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ch_PosTargetZone.Location = new System.Drawing.Point(4, 161);
            this.ch_PosTargetZone.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ch_PosTargetZone.Name = "ch_PosTargetZone";
            this.ch_PosTargetZone.Size = new System.Drawing.Size(250, 45);
            this.ch_PosTargetZone.TabIndex = 6;
            this.ch_PosTargetZone.Text = "Zona objetivo";
            this.ch_PosTargetZone.UseVisualStyleBackColor = true;
            this.ch_PosTargetZone.CheckedChanged += new System.EventHandler(this.ch_TargetZone_CheckedChanged);
            // 
            // cb_posTargets
            // 
            this.cb_posTargets.AutoSize = true;
            this.tableLayoutPanel21.SetColumnSpan(this.cb_posTargets, 2);
            this.cb_posTargets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_posTargets.Enabled = false;
            this.cb_posTargets.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_posTargets.Location = new System.Drawing.Point(4, 109);
            this.cb_posTargets.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cb_posTargets.Name = "cb_posTargets";
            this.cb_posTargets.Size = new System.Drawing.Size(250, 42);
            this.cb_posTargets.TabIndex = 24;
            this.cb_posTargets.Text = "Objetivos";
            this.cb_posTargets.UseVisualStyleBackColor = true;
            this.cb_posTargets.CheckedChanged += new System.EventHandler(this.cb_posTargets_CheckedChanged);
            // 
            // lblFocus
            // 
            this.lblFocus.AutoSize = true;
            this.lblFocus.Location = new System.Drawing.Point(417, 0);
            this.lblFocus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFocus.Name = "lblFocus";
            this.lblFocus.Size = new System.Drawing.Size(0, 20);
            this.lblFocus.TabIndex = 31;
            // 
            // tableLayoutPanel14
            // 
            this.tableLayoutPanel14.ColumnCount = 4;
            this.tableLayoutPanel13.SetColumnSpan(this.tableLayoutPanel14, 2);
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel14.Controls.Add(this.cb_PosX, 0, 1);
            this.tableLayoutPanel14.Controls.Add(this.cb_PosY, 2, 1);
            this.tableLayoutPanel14.Controls.Add(this.cb_PosTrajectory, 0, 2);
            this.tableLayoutPanel14.Controls.Add(this.label9, 0, 0);
            this.tableLayoutPanel14.Controls.Add(this.sampleSelector, 2, 2);
            this.tableLayoutPanel14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel14.Location = new System.Drawing.Point(4, 5);
            this.tableLayoutPanel14.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel14.Name = "tableLayoutPanel14";
            this.tableLayoutPanel14.RowCount = 3;
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel14.Size = new System.Drawing.Size(518, 148);
            this.tableLayoutPanel14.TabIndex = 6;
            // 
            // cb_PosX
            // 
            this.cb_PosX.AutoSize = true;
            this.tableLayoutPanel14.SetColumnSpan(this.cb_PosX, 2);
            this.cb_PosX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_PosX.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_PosX.Location = new System.Drawing.Point(4, 54);
            this.cb_PosX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cb_PosX.Name = "cb_PosX";
            this.cb_PosX.Size = new System.Drawing.Size(198, 39);
            this.cb_PosX.TabIndex = 23;
            this.cb_PosX.Text = "Eje X";
            this.cb_PosX.UseVisualStyleBackColor = true;
            this.cb_PosX.CheckedChanged += new System.EventHandler(this.cb_PosX_CheckedChanged);
            // 
            // cb_PosY
            // 
            this.cb_PosY.AutoSize = true;
            this.tableLayoutPanel14.SetColumnSpan(this.cb_PosY, 2);
            this.cb_PosY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_PosY.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_PosY.Location = new System.Drawing.Point(210, 54);
            this.cb_PosY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cb_PosY.Name = "cb_PosY";
            this.cb_PosY.Size = new System.Drawing.Size(304, 39);
            this.cb_PosY.TabIndex = 24;
            this.cb_PosY.Text = "Eje Y";
            this.cb_PosY.UseVisualStyleBackColor = true;
            this.cb_PosY.CheckedChanged += new System.EventHandler(this.cb_PosY_CheckedChanged);
            // 
            // cb_PosTrajectory
            // 
            this.cb_PosTrajectory.AutoSize = true;
            this.cb_PosTrajectory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_PosTrajectory.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_PosTrajectory.Location = new System.Drawing.Point(4, 103);
            this.cb_PosTrajectory.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cb_PosTrajectory.Name = "cb_PosTrajectory";
            this.cb_PosTrajectory.Size = new System.Drawing.Size(147, 40);
            this.cb_PosTrajectory.TabIndex = 25;
            this.cb_PosTrajectory.Text = "Trayectoria";
            this.cb_PosTrajectory.UseVisualStyleBackColor = true;
            this.cb_PosTrajectory.CheckedChanged += new System.EventHandler(this.cb_PosTrajectory_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.tableLayoutPanel14.SetColumnSpan(this.label9, 3);
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(4, 0);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(353, 49);
            this.label9.TabIndex = 30;
            this.label9.Text = "Posición";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // sampleSelector
            // 
            this.sampleSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sampleSelector.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sampleSelector.Location = new System.Drawing.Point(210, 103);
            this.sampleSelector.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.sampleSelector.Name = "sampleSelector";
            this.sampleSelector.Size = new System.Drawing.Size(147, 28);
            this.sampleSelector.TabIndex = 31;
            this.sampleSelector.Visible = false;
            // 
            // groupBox3
            // 
            this.tableLayoutPanel13.SetColumnSpan(this.groupBox3, 2);
            this.groupBox3.Controls.Add(this.tableLayoutPanel15);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Enabled = false;
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(4, 384);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Size = new System.Drawing.Size(518, 243);
            this.groupBox3.TabIndex = 37;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Trials";
            // 
            // tableLayoutPanel15
            // 
            this.tableLayoutPanel15.ColumnCount = 7;
            this.tableLayoutPanel15.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel15.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel15.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel15.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel15.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel15.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel15.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel15.Controls.Add(this.cb_error_flag, 2, 5);
            this.tableLayoutPanel15.Controls.Add(this.label1, 1, 5);
            this.tableLayoutPanel15.Controls.Add(this.cb_TrialsSegmention, 1, 1);
            this.tableLayoutPanel15.Controls.Add(this.label15, 1, 3);
            this.tableLayoutPanel15.Controls.Add(this.trialSelector, 2, 1);
            this.tableLayoutPanel15.Controls.Add(this.btnApplyTrials, 4, 3);
            this.tableLayoutPanel15.Controls.Add(this.label16, 1, 4);
            this.tableLayoutPanel15.Controls.Add(this.cb_initial_flag, 2, 3);
            this.tableLayoutPanel15.Controls.Add(this.cb_final_flag, 2, 4);
            this.tableLayoutPanel15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel15.Location = new System.Drawing.Point(4, 26);
            this.tableLayoutPanel15.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel15.Name = "tableLayoutPanel15";
            this.tableLayoutPanel15.RowCount = 7;
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel15.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel15.Size = new System.Drawing.Size(510, 212);
            this.tableLayoutPanel15.TabIndex = 9;
            // 
            // cb_error_flag
            // 
            this.cb_error_flag.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_error_flag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_error_flag.Enabled = false;
            this.cb_error_flag.FormattingEnabled = true;
            this.cb_error_flag.Location = new System.Drawing.Point(156, 151);
            this.cb_error_flag.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cb_error_flag.Name = "cb_error_flag";
            this.cb_error_flag.Size = new System.Drawing.Size(145, 30);
            this.cb_error_flag.TabIndex = 38;
            this.cb_error_flag.SelectedIndexChanged += new System.EventHandler(this.cb_error_flag_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(29, 146);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 42);
            this.label1.TabIndex = 37;
            this.label1.Text = "Flag Error:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cb_TrialsSegmention
            // 
            this.cb_TrialsSegmention.AutoSize = true;
            this.cb_TrialsSegmention.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_TrialsSegmention.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_TrialsSegmention.Location = new System.Drawing.Point(29, 15);
            this.cb_TrialsSegmention.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cb_TrialsSegmention.Name = "cb_TrialsSegmention";
            this.cb_TrialsSegmention.Size = new System.Drawing.Size(119, 32);
            this.cb_TrialsSegmention.TabIndex = 32;
            this.cb_TrialsSegmention.Text = "Separar";
            this.cb_TrialsSegmention.UseVisualStyleBackColor = true;
            this.cb_TrialsSegmention.CheckedChanged += new System.EventHandler(this.cb_TrialsSegmention_CheckedChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(29, 62);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(119, 42);
            this.label15.TabIndex = 33;
            this.label15.Text = "Flag Inicio:";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // trialSelector
            // 
            this.trialSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trialSelector.Enabled = false;
            this.trialSelector.Location = new System.Drawing.Point(156, 15);
            this.trialSelector.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.trialSelector.Name = "trialSelector";
            this.trialSelector.Size = new System.Drawing.Size(145, 28);
            this.trialSelector.TabIndex = 7;
            this.trialSelector.ValueChanged += new System.EventHandler(this.trialSelector_ValueChanged);
            // 
            // btnApplyTrials
            // 
            this.tableLayoutPanel15.SetColumnSpan(this.btnApplyTrials, 2);
            this.btnApplyTrials.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnApplyTrials.Enabled = false;
            this.btnApplyTrials.Location = new System.Drawing.Point(360, 67);
            this.btnApplyTrials.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnApplyTrials.Name = "btnApplyTrials";
            this.tableLayoutPanel15.SetRowSpan(this.btnApplyTrials, 2);
            this.btnApplyTrials.Size = new System.Drawing.Size(119, 74);
            this.btnApplyTrials.TabIndex = 31;
            this.btnApplyTrials.Text = "Aplicar";
            this.btnApplyTrials.UseVisualStyleBackColor = true;
            this.btnApplyTrials.Click += new System.EventHandler(this.btnApplyTrials_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(29, 104);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(119, 42);
            this.label16.TabIndex = 34;
            this.label16.Text = "Flag Final:";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cb_initial_flag
            // 
            this.cb_initial_flag.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_initial_flag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_initial_flag.Enabled = false;
            this.cb_initial_flag.FormattingEnabled = true;
            this.cb_initial_flag.Location = new System.Drawing.Point(156, 67);
            this.cb_initial_flag.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cb_initial_flag.Name = "cb_initial_flag";
            this.cb_initial_flag.Size = new System.Drawing.Size(145, 30);
            this.cb_initial_flag.TabIndex = 35;
            this.cb_initial_flag.SelectedIndexChanged += new System.EventHandler(this.cb_initial_flag_SelectedIndexChanged);
            // 
            // cb_final_flag
            // 
            this.cb_final_flag.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_final_flag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_final_flag.Enabled = false;
            this.cb_final_flag.FormattingEnabled = true;
            this.cb_final_flag.Location = new System.Drawing.Point(156, 109);
            this.cb_final_flag.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cb_final_flag.Name = "cb_final_flag";
            this.cb_final_flag.Size = new System.Drawing.Size(145, 30);
            this.cb_final_flag.TabIndex = 36;
            this.cb_final_flag.SelectedIndexChanged += new System.EventHandler(this.cb_final_flag_SelectedIndexChanged);
            // 
            // PanelInfoPosition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel10);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "PanelInfoPosition";
            this.Size = new System.Drawing.Size(1503, 632);
            this.tableLayoutPanel10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartData)).EndInit();
            this.tableLayoutPanel13.ResumeLayout(false);
            this.tableLayoutPanel21.ResumeLayout(false);
            this.tableLayoutPanel21.PerformLayout();
            this.tableLayoutPanel14.ResumeLayout(false);
            this.tableLayoutPanel14.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sampleSelector)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.tableLayoutPanel15.ResumeLayout(false);
            this.tableLayoutPanel15.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trialSelector)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartData;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel13;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel21;
        private System.Windows.Forms.CheckBox cb_PosIdeal;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox ch_PosTargetZone;
        private System.Windows.Forms.CheckBox cb_posTargets;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel14;
        private System.Windows.Forms.CheckBox cb_PosX;
        private System.Windows.Forms.CheckBox cb_PosY;
        private System.Windows.Forms.CheckBox cb_PosTrajectory;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel15;
        private System.Windows.Forms.CheckBox cb_TrialsSegmention;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown trialSelector;
        private System.Windows.Forms.Button btnApplyTrials;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cb_initial_flag;
        private System.Windows.Forms.ComboBox cb_final_flag;
        private System.Windows.Forms.Label lblFocus;
        private System.Windows.Forms.ComboBox cb_error_flag;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown sampleSelector;
    }
}
