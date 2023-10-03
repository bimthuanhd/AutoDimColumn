using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SingleData;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HTAddin
{
    public static class TColumnUtil
    {
        public static int GetColumnIndex(this TColumn q)
        {
            return q.Dict.TColumns.IndexOf(q);
        }
        
     
       
       

    }
}
