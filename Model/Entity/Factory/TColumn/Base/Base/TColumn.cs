using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using SingleData;

namespace HTAddin
{
    public class TColumn
    {
        public Element Column { get; set; }
        public TColumnGroup Dict { get; set; }

        private int? tColumnIndex;
        public int TColumnIndex
        {
            get
            {
                if (tColumnIndex == null)
                {
                    tColumnIndex = TColumnUtil.GetColumnIndex(this);
                }
                return this.tColumnIndex.Value;
            }
        }

        private string type;
        public string Type
        {
            get => this.type ?? (this.type = TColumnUtil.GetType(this));
            set => this.type = value;
        }

        private TDimGroup tDimGroup;
        public TDimGroup TDimGroup
        {
            get => this.tDimGroup ?? (this.tDimGroup = TColumnUtil.GetTDimGroup(this));
            set => this.tDimGroup = value;
        }


    }

}
