using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using SingleData;

namespace HTAddin
{
    public static class TColumnVMUtil
    {
        public static TColumnVM GetVM(this TColumn tColumn, TColumnGroupVM dict)
        {
            var newTcolumnVM = new TColumnVM
            {
                ModelItem = tColumn,
                Dict = dict
            };

            return newTcolumnVM;
        }

    }
}
