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
    public class TDimGroup
    {
        public TWallColumn TWallColumn;
        private List<TDim> detailTDims;
        public List<TDim> DetailTDims
        {
            get => this.detailTDims ?? (this.detailTDims = TDimGroupUtil.GetDetailTDims(this));
            set => this.detailTDims = value;
        }
        private List<TDim> totalTDims;
        public List<TDim> TotalTDims
        {
            get => this.totalTDims ?? (this.totalTDims = TDimGroupUtil.GetTotalTDims(this));
            set => this.totalTDims = value;
        }
        public ReferenceArray RA { get; set; }
        public List<XYZ> Points { get; set; }
        public List<XYZ> Directions { get; set; }
        public List<Face> Faces { get; set; }

    }
}
