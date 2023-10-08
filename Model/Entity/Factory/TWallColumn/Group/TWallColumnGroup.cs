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
    public class TWallColumnGroup
    {
        public List<Element> WallColumns { get; set; }

        private List<TWallColumn> tWallColumns;
        public List<TWallColumn> TWallColumns
        {
            get => this.tWallColumns ?? (this.tWallColumns = new List<TWallColumn>());
            set => this.tWallColumns = value;
        }

        private List<Grid> projectGrids;
        public List<Grid> ProjectGrids
        {
            get => this.projectGrids ?? (this.projectGrids = TWallColumnGroupUtil.GetProjectGrids(this));
            set => this.projectGrids = value;
        }

        private Input input;
        public Input Input
        {
            get => this.input ?? (this.input = new Input() { TWallColumnGroup = this });
            set => this.input = value;
        }
    }
}
