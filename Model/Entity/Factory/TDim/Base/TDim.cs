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
    public class TDim
    {
        public TDimGroup Dict;
        private TGrid nearestTGrid;
        public TGrid NearestTGrid
        {
            get => this.nearestTGrid ?? (this.nearestTGrid = TDimUtil.GetNearestTGrid(this));
            set=> this.nearestTGrid = value;
        }
        private Line line;
        public Line Line
        {
            get => this.line ?? (this.line = TDimUtil.GetLine(this));
            set => this.line = value;
        }
        public ReferenceArray RA { get; set; }
        public List<XYZ> TargetPoint { get; set; }
        public XYZ Direction { get; set; }
        public Face Face { get; set; }

    }

}
