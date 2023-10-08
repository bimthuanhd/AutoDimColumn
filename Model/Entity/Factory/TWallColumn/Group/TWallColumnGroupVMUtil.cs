using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleData;

namespace HTAddin
{
    public static class TWallColumnGroupVMUtil
    {
        public static TWallColumnGroupVM GetVM(this TWallColumnGroup q)
        {
            return new TWallColumnGroupVM
            {
                ModelItem = q
            };
        }

        public static ObservableCollection<TWallColumnVM> GetTWallColumnsVM(this TWallColumnGroupVM q)
        {
            return q.ModelItem.TWallColumns.Select(x => x.GetVM(q)).ToObservableCollection();
        }

        public static InputVM GetInputVM(this TWallColumnGroupVM q)
        {
            return InputVMUtil.GetVM(q.ModelItem.Input, q);
        }
    }
}
