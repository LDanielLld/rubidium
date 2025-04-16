namespace REVIREPanels
{
    partial class PanelEstadisticas
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutViewData = new System.Windows.Forms.TableLayoutPanel();
            this.tabChartData = new System.Windows.Forms.TabControl();
            this.tabInfoPos = new System.Windows.Forms.TabPage();
            this.panelInfoPosition = new REVIREPanels.Estadisticas.Paneles.PanelInfoPosition();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutViewData.SuspendLayout();
            this.tabChartData.SuspendLayout();
            this.tabInfoPos.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1500, 1000);
            this.panel1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1500, 1000);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tableLayoutViewData);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(4, 4);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(1492, 992);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "RESULTADOS";
            // 
            // tableLayoutViewData
            // 
            this.tableLayoutViewData.ColumnCount = 3;
            this.tableLayoutViewData.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutViewData.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.tableLayoutViewData.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutViewData.Controls.Add(this.tabChartData, 1, 0);
            this.tableLayoutViewData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutViewData.Location = new System.Drawing.Point(4, 27);
            this.tableLayoutViewData.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutViewData.Name = "tableLayoutViewData";
            this.tableLayoutViewData.RowCount = 3;
            this.tableLayoutViewData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.tableLayoutViewData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 96F));
            this.tableLayoutViewData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.tableLayoutViewData.Size = new System.Drawing.Size(1484, 961);
            this.tableLayoutViewData.TabIndex = 4;
            // 
            // tabChartData
            // 
            this.tabChartData.Controls.Add(this.tabInfoPos);
            this.tabChartData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabChartData.Location = new System.Drawing.Point(80, 6);
            this.tabChartData.Margin = new System.Windows.Forms.Padding(6);
            this.tabChartData.Name = "tabChartData";
            this.tableLayoutViewData.SetRowSpan(this.tabChartData, 3);
            this.tabChartData.SelectedIndex = 0;
            this.tabChartData.Size = new System.Drawing.Size(1323, 949);
            this.tabChartData.TabIndex = 10;
            // 
            // tabInfoPos
            // 
            this.tabInfoPos.BackColor = System.Drawing.Color.White;
            this.tabInfoPos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabInfoPos.Controls.Add(this.panelInfoPosition);
            this.tabInfoPos.Location = new System.Drawing.Point(4, 34);
            this.tabInfoPos.Margin = new System.Windows.Forms.Padding(4);
            this.tabInfoPos.Name = "tabInfoPos";
            this.tabInfoPos.Padding = new System.Windows.Forms.Padding(4);
            this.tabInfoPos.Size = new System.Drawing.Size(1315, 911);
            this.tabInfoPos.TabIndex = 0;
            this.tabInfoPos.Text = "tabInfoPos";
            // 
            // panelInfoPosition
            // 
            this.panelInfoPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelInfoPosition.IsSelectedPie = false;
            this.panelInfoPosition.Location = new System.Drawing.Point(4, 4);
            this.panelInfoPosition.Margin = new System.Windows.Forms.Padding(9, 8, 9, 8);
            this.panelInfoPosition.Name = "panelInfoPosition";
            this.panelInfoPosition.Size = new System.Drawing.Size(1305, 901);
            this.panelInfoPosition.TabIndex = 0;
            // 
            // PanelEstadisticas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PanelEstadisticas";
            this.Size = new System.Drawing.Size(1500, 1000);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutViewData.ResumeLayout(false);
            this.tabChartData.ResumeLayout(false);
            this.tabInfoPos.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutViewData;
        private System.Windows.Forms.TabControl tabChartData;
        private System.Windows.Forms.TabPage tabInfoPos;
        private Estadisticas.Paneles.PanelInfoPosition panelInfoPosition;
    }
}
