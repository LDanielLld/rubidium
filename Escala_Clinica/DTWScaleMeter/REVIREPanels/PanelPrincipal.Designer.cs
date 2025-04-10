namespace REVIREPanels
{
    partial class PanelPrincipal
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
            this.panelREVIRE = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelREVIRE
            // 
            this.panelREVIRE.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelREVIRE.Location = new System.Drawing.Point(0, 0);
            this.panelREVIRE.Margin = new System.Windows.Forms.Padding(0);
            this.panelREVIRE.Name = "panelREVIRE";
            this.panelREVIRE.Size = new System.Drawing.Size(1000, 600);
            this.panelREVIRE.TabIndex = 0;
            // 
            // PanelPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelREVIRE);
            this.Name = "PanelPrincipal";
            this.Size = new System.Drawing.Size(1000, 600);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelREVIRE;
    }
}
