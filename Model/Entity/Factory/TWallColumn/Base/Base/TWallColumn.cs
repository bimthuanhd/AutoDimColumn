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
    public class TWallColumn
    {
        public Element WallColumn { get; set; }
        public TWallColumnGroup Dict { get; set; }

        private int? tWallColumnIndex;
        public int TWallColumnIndex
        {
            get
            {
                if (tWallColumnIndex == null)
                {
                    tWallColumnIndex = TWallColumnUtil.GetWallColumnIndex(this);
                }
                return this.tWallColumnIndex.Value;
            }
        }

        private string type;
        public string Type
        {
            get => this.type ?? (this.type = TWallColumnUtil.GetType(this));
            set => this.type = value;
        }

        private TDimGroup tDimGroup;
        public TDimGroup TDimGroup
        {
            get => this.tDimGroup ?? (this.tDimGroup = TWallColumnUtil.GetTDimGroup(this));
            set => this.tDimGroup = value;
        }


    }

}
