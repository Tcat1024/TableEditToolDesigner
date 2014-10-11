using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;

namespace PUB.STCT.Client
{
    public class EditBoxDesigner:System.Windows.Forms.Design.ControlDesigner
    {
        public EditBoxDesigner()
        {

        }
        public override SelectionRules SelectionRules
        {
            get
            {
                SelectionRules rules = SelectionRules.Visible | SelectionRules.Moveable | SelectionRules.LeftSizeable | SelectionRules.RightSizeable;
                return rules;
            }
        }
    }
}
