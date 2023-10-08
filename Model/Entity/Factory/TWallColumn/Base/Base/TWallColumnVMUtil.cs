using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using SingleData;

namespace HTAddin
{
    public static class TWallColumnVMUtil
    {
        public static TWallColumnVM GetVM(this TWallColumn tWallColumn, TWallColumnGroupVM dict)
        {
            var newTWallColumnVM = new TWallColumnVM
            {
                ModelItem = tWallColumn,
                Dict = dict
            };

            return newTWallColumnVM;
        }

    }
}
