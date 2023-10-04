using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleData;

namespace HTAddin
{
    public class TColumnVM : BaseViewModel
    {
        public TColumn ModelItem { get; set; }

        public TColumnGroupVM Dict { get; set; }

        public int TColumnIndex
        {
            get => this.ModelItem.TColumnIndex;
        }
        public TDimGroup TDimGroupVM
        {
            get => this.ModelItem.TDimGroup;
            set => this.ModelItem.TDimGroup = value;
        }


    }
}
