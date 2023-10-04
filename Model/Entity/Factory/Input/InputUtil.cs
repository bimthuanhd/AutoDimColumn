using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using SingleData;

namespace HTAddin
{
    public static class InputUtil
    {
        public static List<DimensionType> GetDimensionTypes (Input q)
        {
            var dimensionTypes = new List<DimensionType>();
            dimensionTypes = Util.GetAllDimensionTypes(Data.Instance.doc);

            return dimensionTypes;
        }

        public static List<string> GetItemSource_NameDimStyle(Input q)
        {
            var itemSources = new List<string>();
            foreach (var dimensionType in q.DimensionTypes)
            {
                var dimName = dimensionType.Name;
                itemSources.Add(dimName);
            }

            return itemSources;
        }
    }
}
