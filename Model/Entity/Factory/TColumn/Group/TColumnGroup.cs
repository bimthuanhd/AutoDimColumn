using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleData;
using Autodesk.Revit.UI;

namespace HTAddin
{
    public class TColumnGroup
    {
        public List<Element> Columns { get; set; }

        private List<TColumn> tColumn;
        public List<TColumn> TColumns
        {
            get => this.tColumn ?? (this.tColumn = new List<TColumn>());
            set => this.tColumn = value;
        }

        private List<Grid> projectGrids;
        public List<Grid> ProjectGrids
        {
            get => this.projectGrids ?? (this.projectGrids = TColumnGroupUtil.GetProjectGrids(this));
            set => this.projectGrids = value;
        }

        private Input input;
        public Input Input
        {
            get => this.input ?? (this.input = new Input() { TColumnGroup = this });
            set => this.input = value;
        }
    }
}
