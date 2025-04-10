namespace REVIREPanels.Componentes
{
    partial class PanelForceOrVel
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
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.trackBarLeft = new System.Windows.Forms.TrackBar();
            this.trackBarRight = new System.Windows.Forms.TrackBar();
            this.trackBarBack = new System.Windows.Forms.TrackBar();
            this.trackBarForward = new System.Windows.Forms.TrackBar();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblBack = new System.Windows.Forms.Label();
            this.btnSubsBack = new System.Windows.Forms.Button();
            this.btnAddBack = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblLeft = new System.Windows.Forms.Label();
            this.btnSubsLeft = new System.Windows.Forms.Button();
            this.btnAddLeft = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lblForward = new System.Windows.Forms.Label();
            this.btnSubsForward = new System.Windows.Forms.Button();
            this.btnAddForward = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.lblRight = new System.Windows.Forms.Label();
            this.btnSubsRight = new System.Windows.Forms.Button();
            this.btnAddRight = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarForward)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 3;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableLayoutPanel6.Controls.Add(this.trackBarLeft, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.trackBarRight, 2, 1);
            this.tableLayoutPanel6.Controls.Add(this.trackBarBack, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.trackBarForward, 1, 2);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel3, 2, 2);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel4, 2, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 3;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42.5F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42.5F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(765, 745);
            this.tableLayoutPanel6.TabIndex = 8;
            // 
            // trackBarLeft
            // 
            this.trackBarLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(223)))), ((int)(((byte)(240)))));
            this.trackBarLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarLeft.LargeChange = 1;
            this.trackBarLeft.Location = new System.Drawing.Point(4, 321);
            this.trackBarLeft.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.trackBarLeft.Maximum = 20;
            this.trackBarLeft.Name = "trackBarLeft";
            this.trackBarLeft.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.trackBarLeft.Size = new System.Drawing.Size(336, 101);
            this.trackBarLeft.TabIndex = 0;
            this.trackBarLeft.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBarLeft.Scroll += new System.EventHandler(this.trackBarLeft_Scroll);
            // 
            // trackBarRight
            // 
            this.trackBarRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(223)))), ((int)(((byte)(240)))));
            this.trackBarRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarRight.LargeChange = 1;
            this.trackBarRight.Location = new System.Drawing.Point(424, 321);
            this.trackBarRight.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.trackBarRight.Maximum = 20;
            this.trackBarRight.Name = "trackBarRight";
            this.trackBarRight.Size = new System.Drawing.Size(337, 101);
            this.trackBarRight.TabIndex = 1;
            this.trackBarRight.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBarRight.Scroll += new System.EventHandler(this.trackBarRight_Scroll);
            // 
            // trackBarBack
            // 
            this.trackBarBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(223)))), ((int)(((byte)(240)))));
            this.trackBarBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarBack.LargeChange = 1;
            this.trackBarBack.Location = new System.Drawing.Point(348, 5);
            this.trackBarBack.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.trackBarBack.Maximum = 20;
            this.trackBarBack.Name = "trackBarBack";
            this.trackBarBack.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarBack.Size = new System.Drawing.Size(68, 306);
            this.trackBarBack.TabIndex = 2;
            this.trackBarBack.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBarBack.Scroll += new System.EventHandler(this.trackBarBack_Scroll);
            // 
            // trackBarForward
            // 
            this.trackBarForward.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(223)))), ((int)(((byte)(240)))));
            this.trackBarForward.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarForward.LargeChange = 1;
            this.trackBarForward.Location = new System.Drawing.Point(348, 432);
            this.trackBarForward.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.trackBarForward.Maximum = 20;
            this.trackBarForward.Name = "trackBarForward";
            this.trackBarForward.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarForward.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.trackBarForward.Size = new System.Drawing.Size(68, 308);
            this.trackBarForward.TabIndex = 3;
            this.trackBarForward.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBarForward.Value = 20;
            this.trackBarForward.Scroll += new System.EventHandler(this.trackBarForward_Scroll);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.Controls.Add(this.lblBack, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSubsBack, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnAddBack, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label6, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(344, 316);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // lblBack
            // 
            this.lblBack.AutoSize = true;
            this.lblBack.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblBack.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBack.Location = new System.Drawing.Point(175, 0);
            this.lblBack.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBack.Name = "lblBack";
            this.lblBack.Size = new System.Drawing.Size(129, 63);
            this.lblBack.TabIndex = 0;
            this.lblBack.Text = "0";
            this.lblBack.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSubsBack
            // 
            this.btnSubsBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSubsBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubsBack.Location = new System.Drawing.Point(137, 0);
            this.btnSubsBack.Margin = new System.Windows.Forms.Padding(0);
            this.btnSubsBack.Name = "btnSubsBack";
            this.btnSubsBack.Size = new System.Drawing.Size(34, 63);
            this.btnSubsBack.TabIndex = 1;
            this.btnSubsBack.Text = "-";
            this.btnSubsBack.UseVisualStyleBackColor = true;
            this.btnSubsBack.Click += new System.EventHandler(this.btnSubsBack_Click);
            // 
            // btnAddBack
            // 
            this.btnAddBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddBack.Location = new System.Drawing.Point(308, 0);
            this.btnAddBack.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddBack.Name = "btnAddBack";
            this.btnAddBack.Size = new System.Drawing.Size(36, 63);
            this.btnAddBack.TabIndex = 2;
            this.btnAddBack.Text = "+";
            this.btnAddBack.UseVisualStyleBackColor = true;
            this.btnAddBack.Click += new System.EventHandler(this.btnAddBack_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label6, 3);
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(141, 63);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(199, 63);
            this.label6.TabIndex = 3;
            this.label6.Text = "ATRÁS";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel2.Controls.Add(this.lblLeft, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnSubsLeft, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnAddLeft, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 427);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(344, 318);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // lblLeft
            // 
            this.lblLeft.AutoSize = true;
            this.lblLeft.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLeft.Location = new System.Drawing.Point(38, 0);
            this.lblLeft.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLeft.Name = "lblLeft";
            this.lblLeft.Size = new System.Drawing.Size(129, 63);
            this.lblLeft.TabIndex = 0;
            this.lblLeft.Text = "0";
            this.lblLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSubsLeft
            // 
            this.btnSubsLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSubsLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubsLeft.Location = new System.Drawing.Point(0, 0);
            this.btnSubsLeft.Margin = new System.Windows.Forms.Padding(0);
            this.btnSubsLeft.Name = "btnSubsLeft";
            this.btnSubsLeft.Size = new System.Drawing.Size(34, 63);
            this.btnSubsLeft.TabIndex = 1;
            this.btnSubsLeft.Text = "-";
            this.btnSubsLeft.UseVisualStyleBackColor = true;
            this.btnSubsLeft.Click += new System.EventHandler(this.btnAddLeft_Click);
            // 
            // btnAddLeft
            // 
            this.btnAddLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddLeft.Location = new System.Drawing.Point(171, 0);
            this.btnAddLeft.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddLeft.Name = "btnAddLeft";
            this.btnAddLeft.Size = new System.Drawing.Size(34, 63);
            this.btnAddLeft.TabIndex = 2;
            this.btnAddLeft.Text = "+";
            this.btnAddLeft.UseVisualStyleBackColor = true;
            this.btnAddLeft.Click += new System.EventHandler(this.btnAddLeft_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.label7, 3);
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(4, 63);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(197, 63);
            this.label7.TabIndex = 3;
            this.label7.Text = "IZQUIERDA";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel3.Controls.Add(this.lblForward, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.btnSubsForward, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.btnAddForward, 2, 2);
            this.tableLayoutPanel3.Controls.Add(this.label8, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(420, 427);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(345, 318);
            this.tableLayoutPanel3.TabIndex = 6;
            // 
            // lblForward
            // 
            this.lblForward.AutoSize = true;
            this.lblForward.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblForward.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblForward.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblForward.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblForward.Location = new System.Drawing.Point(38, 253);
            this.lblForward.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblForward.Name = "lblForward";
            this.lblForward.Size = new System.Drawing.Size(130, 65);
            this.lblForward.TabIndex = 0;
            this.lblForward.Text = "0";
            this.lblForward.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSubsForward
            // 
            this.btnSubsForward.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSubsForward.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubsForward.Location = new System.Drawing.Point(0, 253);
            this.btnSubsForward.Margin = new System.Windows.Forms.Padding(0);
            this.btnSubsForward.Name = "btnSubsForward";
            this.btnSubsForward.Size = new System.Drawing.Size(34, 65);
            this.btnSubsForward.TabIndex = 1;
            this.btnSubsForward.Text = "-";
            this.btnSubsForward.UseVisualStyleBackColor = true;
            this.btnSubsForward.Click += new System.EventHandler(this.btnSubsForward_Click);
            // 
            // btnAddForward
            // 
            this.btnAddForward.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddForward.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddForward.Location = new System.Drawing.Point(172, 253);
            this.btnAddForward.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddForward.Name = "btnAddForward";
            this.btnAddForward.Size = new System.Drawing.Size(34, 65);
            this.btnAddForward.TabIndex = 2;
            this.btnAddForward.Text = "+";
            this.btnAddForward.UseVisualStyleBackColor = true;
            this.btnAddForward.Click += new System.EventHandler(this.btnAddForward_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.tableLayoutPanel3.SetColumnSpan(this.label8, 3);
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(4, 190);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(198, 63);
            this.label8.TabIndex = 3;
            this.label8.Text = "ADELANTE";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 4;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel4.Controls.Add(this.lblRight, 2, 2);
            this.tableLayoutPanel4.Controls.Add(this.btnSubsRight, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.btnAddRight, 3, 2);
            this.tableLayoutPanel4.Controls.Add(this.label5, 1, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(420, 0);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(345, 316);
            this.tableLayoutPanel4.TabIndex = 7;
            // 
            // lblRight
            // 
            this.lblRight.AutoSize = true;
            this.lblRight.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRight.Location = new System.Drawing.Point(176, 252);
            this.lblRight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRight.Name = "lblRight";
            this.lblRight.Size = new System.Drawing.Size(130, 64);
            this.lblRight.TabIndex = 0;
            this.lblRight.Text = "0";
            this.lblRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSubsRight
            // 
            this.btnSubsRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSubsRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubsRight.Location = new System.Drawing.Point(138, 252);
            this.btnSubsRight.Margin = new System.Windows.Forms.Padding(0);
            this.btnSubsRight.Name = "btnSubsRight";
            this.btnSubsRight.Size = new System.Drawing.Size(34, 64);
            this.btnSubsRight.TabIndex = 1;
            this.btnSubsRight.Text = "-";
            this.btnSubsRight.UseVisualStyleBackColor = true;
            this.btnSubsRight.Click += new System.EventHandler(this.btnSubsRight_Click);
            // 
            // btnAddRight
            // 
            this.btnAddRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddRight.Location = new System.Drawing.Point(310, 252);
            this.btnAddRight.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddRight.Name = "btnAddRight";
            this.btnAddRight.Size = new System.Drawing.Size(35, 64);
            this.btnAddRight.TabIndex = 2;
            this.btnAddRight.Text = "+";
            this.btnAddRight.UseVisualStyleBackColor = true;
            this.btnAddRight.Click += new System.EventHandler(this.btnAddRight_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.tableLayoutPanel4.SetColumnSpan(this.label5, 3);
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(142, 189);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(199, 63);
            this.label5.TabIndex = 3;
            this.label5.Text = "DERECHA";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PanelForceOrVel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel6);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "PanelForceOrVel";
            this.Size = new System.Drawing.Size(765, 745);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarForward)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.TrackBar trackBarLeft;
        private System.Windows.Forms.TrackBar trackBarRight;
        private System.Windows.Forms.TrackBar trackBarBack;
        private System.Windows.Forms.TrackBar trackBarForward;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblBack;
        private System.Windows.Forms.Button btnSubsBack;
        private System.Windows.Forms.Button btnAddBack;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblLeft;
        private System.Windows.Forms.Button btnSubsLeft;
        private System.Windows.Forms.Button btnAddLeft;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label lblForward;
        private System.Windows.Forms.Button btnSubsForward;
        private System.Windows.Forms.Button btnAddForward;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label lblRight;
        private System.Windows.Forms.Button btnSubsRight;
        private System.Windows.Forms.Button btnAddRight;
        private System.Windows.Forms.Label label5;
    }
}
