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
    public class TColumnGroupVM
    {
        public TColumnGroup ModelItem { get; set; }

        private ObservableCollection<TColumnVM> tColumnsVM;
        public ObservableCollection<TColumnVM> TColumnsVM
        {
            get => this.tColumnsVM ?? (this.tColumnsVM = this.GetTColumnsVM());
            set => this.tColumnsVM = value;
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
