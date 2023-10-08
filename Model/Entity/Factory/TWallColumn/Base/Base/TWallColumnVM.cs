using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleData;

namespace HTAddin
{
    public class TWallColumnVM : BaseViewModel
    {
        public TWallColumn ModelItem { get; set; }

        public TWallColumnGroupVM Dict { get; set; }

        public int TColumnIndex
        {
            get => this.ModelItem.TWallColumnIndex;
        }
        public TDimGroup TDimGroupVM
        {
            get => this.ModelItem.TDimGroup;
            set => this.ModelItem.TDimGroup = value;
        }


    }
}
