using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using HTAddin;


namespace SingleData
{
    public static class FormDataUtil
    {
        public static void HandleOkClick(this FormData data)
        {
            FormData formData = FormData.Instance;
            Data Revitdata = Data.Instance;

            var uiApp = data.ModelData.uiApp;
            var app = data.ModelData.app;
            var uiDoc = data.ModelData.uiDoc;
            var doc = data.ModelData.doc;
            Selection sel = data.ModelData.sel;

         
        }

        public static void HandleCancleClick(this FormData data)
        {
        }
    }
}
