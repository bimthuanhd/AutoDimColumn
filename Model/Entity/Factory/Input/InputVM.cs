using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using SingleData;

namespace HTAddin
{
    public class InputVM : BaseViewModel
    {
        public Input ModelItem { get; set; }

        public TColumnGroupVM TColumnGroupVM;

        public bool IsHaveTotalDimVM
        {
            get => ModelItem.IsHaveTotalDim;
            set
            {
                if (this.ModelItem.IsHaveTotalDim != value)
                {
                    this.ModelItem.IsHaveTotalDim = value;
                    OnPropertyChanged(nameof(IsHaveTotalDimVM));
                }
            }
        }
        public List<string> ItemSource_NameDimStyleVM
        {
            get => ModelItem.ItemSource_NameDimStyle;
            set
            {
                if (this.ModelItem.ItemSource_NameDimStyle != value)
                {
                    this.ModelItem.ItemSource_NameDimStyle = value;
                    OnPropertyChanged(nameof(ItemSource_NameDimStyleVM));
                }
            }
        }
        public string Selected_NameDimStyleVM
        {
            get => ModelItem.Selected_NameDimStyle;
            set
            {
                if (this.ModelItem.Selected_NameDimStyle != value)
                {
                    this.ModelItem.Selected_NameDimStyle = value;
                    OnPropertyChanged(nameof(Selected_NameDimStyleVM));
                }
            }
        }

    }
}


