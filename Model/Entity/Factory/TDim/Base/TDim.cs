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
        public TGrid TGrid { get; set; }
        public Line Line { get; set; }  
        public ReferenceArray RA { get; set; } 
        public List<XYZ> TargetPoint { get; set; }
        public XYZ Direction { get; set; }

    }

}
