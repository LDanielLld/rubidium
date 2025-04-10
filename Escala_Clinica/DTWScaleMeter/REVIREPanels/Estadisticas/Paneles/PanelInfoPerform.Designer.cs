namespace REVIREPanels.Estadisticas.Paneles
{
    partial class PanelInfoPerform
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
            this.tableLayoutPanel14 = new System.Windows.Forms.TableLayoutPanel();
            this.cb_Distance = new System.Windows.Forms.CheckBox();
            this.cb_Rep = new System.Windows.Forms.CheckBox();
            this.cb_SuccesFails = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cb_Time = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartData)).BeginInit();
            this.tableLayoutPanel13.SuspendLayout();
            this.tableLayoutPanel14.SuspendLayout();
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
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 444F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 444F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(1002, 411);
            this.tableLayoutPanel10.TabIndex = 11;
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
            this.chartData.Location = new System.Drawing.Point(350, 0);
            this.chartData.Margin = new System.Windows.Forms.Padding(0);
            this.chartData.Name = "chartData";
            this.chartData.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            this.chartData.Size = new System.Drawing.Size(652, 411);
            this.chartData.TabIndex = 7;
            // 
            // tableLayoutPanel13
            // 
            this.tableLayoutPanel13.ColumnCount = 2;
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel13.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel13.Controls.Add(this.tableLayoutPanel14, 0, 0);
            this.tableLayoutPanel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel13.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel13.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel13.Name = "tableLayoutPanel13";
            this.tableLayoutPanel13.RowCount = 2;
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel13.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel13.Size = new System.Drawing.Size(350, 411);
            this.tableLayoutPanel13.TabIndex = 10;
            // 
            // tableLayoutPanel14
            // 
            this.tableLayoutPanel14.ColumnCount = 2;
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel14.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 95F));
            this.tableLayoutPanel14.Controls.Add(this.cb_Distance, 1, 4);
            this.tableLayoutPanel14.Controls.Add(this.cb_Rep, 1, 1);
            this.tableLayoutPanel14.Controls.Add(this.cb_SuccesFails, 1, 2);
            this.tableLayoutPanel14.Controls.Add(this.label9, 1, 0);
            this.tableLayoutPanel14.Controls.Add(this.cb_Time, 1, 3);
            this.tableLayoutPanel14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel14.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel14.Name = "tableLayoutPanel14";
            this.tableLayoutPanel14.RowCount = 5;
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel14.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel14.Size = new System.Drawing.Size(239, 199);
            this.tableLayoutPanel14.TabIndex = 6;
            // 
            // cb_Distance
            // 
            this.cb_Distance.AutoSize = true;
            this.cb_Distance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_Distance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_Distance.Location = new System.Drawing.Point(14, 159);
            this.cb_Distance.Name = "cb_Distance";
            this.cb_Distance.Size = new System.Drawing.Size(222, 37);
            this.cb_Distance.TabIndex = 23;
            this.cb_Distance.Text = "Distancia de Movimiento";
            this.cb_Distance.UseVisualStyleBackColor = true;
            this.cb_Distance.CheckedChanged += new System.EventHandler(this.cb_Distance_CheckedChanged);
            // 
            // cb_Rep
            // 
            this.cb_Rep.AutoSize = true;
            this.cb_Rep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_Rep.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_Rep.Location = new System.Drawing.Point(14, 42);
            this.cb_Rep.Name = "cb_Rep";
            this.cb_Rep.Size = new System.Drawing.Size(222, 33);
            this.cb_Rep.TabIndex = 23;
            this.cb_Rep.Text = "Repeticiones";
            this.cb_Rep.UseVisualStyleBackColor = true;
            this.cb_Rep.CheckedChanged += new System.EventHandler(this.cb_Rep_CheckedChanged);
            // 
            // cb_SuccesFails
            // 
            this.cb_SuccesFails.AutoSize = true;
            this.cb_SuccesFails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_SuccesFails.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_SuccesFails.Location = new System.Drawing.Point(14, 81);
            this.cb_SuccesFails.Name = "cb_SuccesFails";
            this.cb_SuccesFails.Size = new System.Drawing.Size(222, 33);
            this.cb_SuccesFails.TabIndex = 25;
            this.cb_SuccesFails.Text = "Aciertos - Fallos";
            this.cb_SuccesFails.UseVisualStyleBackColor = true;
            this.cb_SuccesFails.CheckedChanged += new System.EventHandler(this.cb_SuccesFails_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(14, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(222, 39);
            this.label9.TabIndex = 30;
            this.label9.Text = "Datos";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cb_Time
            // 
            this.cb_Time.AutoSize = true;
            this.cb_Time.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_Time.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_Time.Location = new System.Drawing.Point(14, 120);
            this.cb_Time.Name = "cb_Time";
            this.cb_Time.Size = new System.Drawing.Size(222, 33);
            this.cb_Time.TabIndex = 24;
            this.cb_Time.Text = "Tiempo";
            this.cb_Time.UseVisualStyleBackColor = true;
            this.cb_Time.CheckedChanged += new System.EventHandler(this.cb_Time_CheckedChanged);
            // 
            // PanelInfoPerform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel10);
            this.Name = "PanelInfoPerform";
            this.Size = new System.Drawing.Size(1002, 411);
            this.tableLayoutPanel10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartData)).EndInit();
            this.tableLayoutPanel13.ResumeLayout(false);
            this.tableLayoutPanel14.ResumeLayout(false);
            this.tableLayoutPanel14.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartData;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel13;
        private System.Windows.Forms.CheckBox cb_Distance;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel14;
        private System.Windows.Forms.CheckBox cb_Rep;
        private System.Windows.Forms.CheckBox cb_Time;
        private System.Windows.Forms.CheckBox cb_SuccesFails;
        private System.Windows.Forms.Label label9;
    }
}
