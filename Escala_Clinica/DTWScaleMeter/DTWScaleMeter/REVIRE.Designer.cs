namespace REVIRE
{
    partial class REVIRE
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

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.linea = new System.Windows.Forms.Panel();
            this.commonLayoutTime = new System.Windows.Forms.TableLayoutPanel();
            this.mainTimer = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.btnCloseAll = new System.Windows.Forms.Button();
            this.timerGeneral = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.commonLayoutTime.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panelMain, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.linea, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.commonLayoutTime, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 91F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1978, 1444);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panelMain
            // 
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 129);
            this.panelMain.Margin = new System.Windows.Forms.Padding(0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1978, 1315);
            this.panelMain.TabIndex = 0;
            // 
            // linea
            // 
            this.linea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linea.Location = new System.Drawing.Point(4, 120);
            this.linea.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.linea.Name = "linea";
            this.linea.Size = new System.Drawing.Size(1970, 4);
            this.linea.TabIndex = 1;
            this.linea.Paint += new System.Windows.Forms.PaintEventHandler(this.linea_Paint);
            // 
            // commonLayoutTime
            // 
            this.commonLayoutTime.ColumnCount = 3;
            this.commonLayoutTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.commonLayoutTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.commonLayoutTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.commonLayoutTime.Controls.Add(this.mainTimer, 2, 0);
            this.commonLayoutTime.Controls.Add(this.lblUserName, 1, 0);
            this.commonLayoutTime.Controls.Add(this.btnCloseAll, 0, 0);
            this.commonLayoutTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commonLayoutTime.Location = new System.Drawing.Point(0, 0);
            this.commonLayoutTime.Margin = new System.Windows.Forms.Padding(0);
            this.commonLayoutTime.Name = "commonLayoutTime";
            this.commonLayoutTime.RowCount = 1;
            this.commonLayoutTime.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.commonLayoutTime.Size = new System.Drawing.Size(1978, 115);
            this.commonLayoutTime.TabIndex = 1;
            // 
            // mainTimer
            // 
            this.mainTimer.AutoSize = true;
            this.mainTimer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainTimer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTimer.Font = new System.Drawing.Font("Arial Narrow", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainTimer.Location = new System.Drawing.Point(1787, 8);
            this.mainTimer.Margin = new System.Windows.Forms.Padding(8);
            this.mainTimer.Name = "mainTimer";
            this.mainTimer.Size = new System.Drawing.Size(183, 99);
            this.mainTimer.TabIndex = 0;
            this.mainTimer.Text = "99:99";
            this.mainTimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserName.Location = new System.Drawing.Point(201, 0);
            this.lblUserName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(1574, 115);
            this.lblUserName.TabIndex = 1;
            this.lblUserName.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // btnCloseAll
            // 
            this.btnCloseAll.Location = new System.Drawing.Point(4, 5);
            this.btnCloseAll.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCloseAll.Name = "btnCloseAll";
            this.btnCloseAll.Size = new System.Drawing.Size(141, 62);
            this.btnCloseAll.TabIndex = 2;
            this.btnCloseAll.Text = "X";
            this.btnCloseAll.UseVisualStyleBackColor = true;
            this.btnCloseAll.Visible = false;
            this.btnCloseAll.Click += new System.EventHandler(this.btnCloseAll_Click);
            // 
            // timerGeneral
            // 
            this.timerGeneral.Enabled = true;
            this.timerGeneral.Tick += new System.EventHandler(this.timerGeneral_Tick);
            // 
            // REVIRE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1978, 1444);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "REVIRE";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "REVIRE";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.commonLayoutTime.ResumeLayout(false);
            this.commonLayoutTime.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.TableLayoutPanel commonLayoutTime;
        private System.Windows.Forms.Label mainTimer;
        private System.Windows.Forms.Timer timerGeneral;
        private System.Windows.Forms.Panel linea;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Button btnCloseAll;
    }
}

