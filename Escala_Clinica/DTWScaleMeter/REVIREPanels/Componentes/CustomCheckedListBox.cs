using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace REVIREPanels.Componentes
{
    class CustomCheckedList : CheckedListBox
    {
        public Color UncheckedColor { get; set; }
        public Color CheckedColor { get; set; }
        public Color IndeterminateColor { get; set; }

       

        //Color checkedItemColor = Color.Green;

        public CustomCheckedList()
        {
            UncheckedColor = Color.Green;
            CheckedColor = Color.Red;
            IndeterminateColor = Color.Orange;

            DoubleBuffered = true;

            
            ItemHeight = 25;

        }

        public override int ItemHeight { get; set; }


        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (this.DesignMode)
            {
                base.OnDrawItem(e);
            }
            else
            {
                Color textColor = this.GetItemCheckState(e.Index) == CheckState.Unchecked ? UncheckedColor : (this.GetItemCheckState(e.Index) == CheckState.Checked ? CheckedColor : IndeterminateColor);

                DrawItemEventArgs e2 = new DrawItemEventArgs
                   (e.Graphics,
                    e.Font,
                    new Rectangle(e.Bounds.Location, e.Bounds.Size),
                    e.Index,
                    (e.State & DrawItemState.Focus) == DrawItemState.Focus ? DrawItemState.None: DrawItemState.None, /* Remove 'selected' state so that the base.OnDrawItem doesn't obliterate the work we are doing here. */
                    textColor,
                    this.BackColor);

                base.OnDrawItem(e2);               

            }
        }

      
    }
}
