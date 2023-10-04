using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleData;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;

namespace HTAddin
{
    public static class TColumnGroupUtil
    {
        //property
        public static TColumnGroup Get(List<FamilyInstance> columns)
        {
            var tColumnGroup = new TColumnGroup();
            tColumnGroup.Columns = columns;
            foreach (var column in columns)
            {
                var tColumn = new TColumn
                {
                    Column = column,
                    Dict = tColumnGroup
                };
                tColumnGroup.TColumns.Add(tColumn);
            }
            return tColumnGroup;
        }

        public static void Do(TColumnGroup q)
        {
            var tColumns = q.TColumns;
            foreach (var tColumn in tColumns)
            {
                var tDims = tColumn.TDimGroup.DetailTDims;
                foreach (var tDim in tDims)
                {
                    TDimUtil.Do(tDim);
                }
            }
        }

        public static List<Grid> GetProjectGrids(TColumnGroup q)
        {
            var doc = Data.Instance.doc;
            var projectGrids = Util.GetAllGrids(doc);

            return projectGrids;
        }



    }

}
