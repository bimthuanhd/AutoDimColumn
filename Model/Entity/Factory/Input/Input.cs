using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using SingleData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAddin
{
    public class Input
    {
        public TColumnGroup TColumnGroup;
        public bool IsHaveTotalDim { get; set; } = true;
        private List<DimensionType> dimensionTypes;

        public List<DimensionType> DimensionTypes
        {
            get => this.dimensionTypes ?? (this.dimensionTypes = InputUtil.GetDimensionTypes(this));
            set => this.dimensionTypes = value;
        }

        private List<string> itemSource_NameDimStyle;
        public List<string> ItemSource_NameDimStyle
        {
            get => this.itemSource_NameDimStyle ?? (this.itemSource_NameDimStyle = InputUtil.GetItemSource_NameDimStyle(this));
            set => this.itemSource_NameDimStyle = value;
        }

        public string Selected_NameDimStyle { get; set; }

        private DimensionType selected_DimStyle;
        public DimensionType Selected_DimStyle
        {
            get => this.selected_DimStyle ?? (this.selected_DimStyle = InputUtil.GetSelected_DimStyle(this));
            set => this.selected_DimStyle = value;
        }

    }
}


