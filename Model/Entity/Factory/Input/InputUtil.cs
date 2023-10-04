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
        
        public static string GetSelected_NameDimStyle(Input q)
        {
            string nameDim = null;
            nameDim = Util.GetDefaultDimensionType(Data.Instance.doc).Name;

            return nameDim;
        }

        public static DimensionType GetSelected_DimStyle( Input q)
        {
            DimensionType dimStyle = null;
            if (q.Selected_NameDimStyle!=null)
            {
                dimStyle = q.DimensionTypes.Where(x => x.Name == q.Selected_NameDimStyle).ToList()[0];
            }

            return dimStyle;
        }
    }
}
