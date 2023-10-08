using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using System.Linq;
using SingleData;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Net;
using System;

namespace HTAddin
{
    [Transaction(TransactionMode.Manual)]
    public class TestCommandCombine : IExternalCommand
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

            List<Element> selected_WallColumns = Util.SelectElementsbyTypes(
                uiDoc,
                new List<BuiltInCategory>() { BuiltInCategory.OST_StructuralColumns,
                BuiltInCategory.OST_Walls });

            List<Element> selected_Walls = selected_WallColumns.Where(x => x.Category.Name == "Walls").ToList();
            //List<FamilyInstance> selected_Columns = selected_WallColumns.Where(x => x.Category.Name == "Structural Columns").ToList();

            data.TWallColumnGroup = TWallColumnGroupUtil.Get(selected_WallColumns);


            Util.ExecuteTransaction(doc, "Testing!", codeToExecute =>
            {
                TWallColumnGroupUtil.Do(data.TWallColumnGroup);
            });

            return Result.Succeeded;
        }
    }

}
