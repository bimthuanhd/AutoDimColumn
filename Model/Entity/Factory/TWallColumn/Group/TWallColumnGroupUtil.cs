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
    public static class TWallColumnGroupUtil
    {
        //property
        public static TWallColumnGroup Get(List<Element> columns)
        {
            var tWallColumnGroup = new TWallColumnGroup();
            tWallColumnGroup.WallColumns = columns;
            foreach (var column in columns)
            {
                var tWallColumn = new TWallColumn
                {
                    WallColumn = column,
                    Dict = tWallColumnGroup
                };
                tWallColumnGroup.TWallColumns.Add(tWallColumn);
            }
            return tWallColumnGroup;
        }

        public static void Do(TWallColumnGroup q)
        {
            var tWallColumns = q.TWallColumns;
            foreach (var tWallColumn in tWallColumns)
            {
                TDimGroupUtil.Do(tWallColumn.TDimGroup);
            }
        }

        public static List<Grid> GetProjectGrids(TWallColumnGroup q)
        {
            var doc = Data.Instance.doc;
            var projectGrids = Util.GetAllGrids(doc);

            return projectGrids;
        }



    }

}
