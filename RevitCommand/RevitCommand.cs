using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleData;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System.Windows;

namespace HTAddin
{
    [Transaction(TransactionMode.Manual)]
    public class RevitCommand : IExternalCommand
    {
        private FormData formData => FormData.Instance;
        private Data data => Data.Instance;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            var app = uiApp.Application;
            Document doc = uiDoc.Document;
            Selection sel = uiDoc.Selection;

            data.uiApp = uiApp;
            data.uiDoc = uiDoc;
            data.app = app;
            data.doc = doc;
            data.sel = sel;

            List<FamilyInstance> selectedColumns = Util.SelectElementsbyType(uiDoc, BuiltInCategory.OST_StructuralColumns);
            data.TColumnGroup = TColumnGroupUtil.Get(selectedColumns);

            foreach (var tColumn in data.TColumnGroup.TColumns)
            {
                var tDimGroup = tColumn.TDimGroup;

                TaskDialog.Show("Show", data.TColumnGroup.TColumns.Count.ToString());
            }

            Util.ExecuteTransaction(doc, "Testing!", codeToExecute =>
            {
                TColumnGroupUtil.Do(data.TColumnGroup);

            });



            return Result.Succeeded;
        }
    }
}
