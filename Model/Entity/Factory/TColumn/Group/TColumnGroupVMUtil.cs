using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleData;

namespace HTAddin
{
    public static class TColumnGroupVMUtil
    {
        public static TColumnGroupVM GetVM(this TColumnGroup q)
        {
            return new TColumnGroupVM
            {
                ModelItem = q
            };
        }

        public static ObservableCollection<TColumnVM> GetTColumnsVM(this TColumnGroupVM q)
        {
            return q.ModelItem.TColumns.Select(x => x.GetVM(q)).ToObservableCollection();
        }

        public static InputVM GetInputVM(this TColumnGroupVM q)
        {
            return InputVMUtil.GetVM(q.ModelItem.Input, q);
        }
    }
}
