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
    public class TWallColumnGroupVM
    {
        public TWallColumnGroup ModelItem { get; set; }

        private ObservableCollection<TWallColumnVM> tWallColumnsVM;
        public ObservableCollection<TWallColumnVM> TWallColumnsVM
        {
            get => this.tWallColumnsVM ?? (this.tWallColumnsVM = this.GetTWallColumnsVM());
            set => this.tWallColumnsVM = value;
        }

        public List<Grid> ProjectGridsVM
        {
            get => this.ModelItem.ProjectGrids;
            set => this.ModelItem.ProjectGrids = value;
        }

        public InputVM inputVM;
        public InputVM InputVM
        {
            get => this.inputVM ?? (this.inputVM = this.GetInputVM());
            set => this.inputVM = value;
        }

    }
}
