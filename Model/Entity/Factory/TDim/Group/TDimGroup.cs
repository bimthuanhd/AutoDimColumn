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
        private List<TDim> tDims;
        public List<TDim> TDims
        {
            get => this.tDims ?? (this.tDims = new List<TDim>());
            set => this.tDims = value;
        }

        public List<Face> Faces { get; set; }


    }
}
