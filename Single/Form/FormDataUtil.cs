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

            var form = data.Form;

            Revitdata.doc = doc;
            Revitdata.sel = sel;

            using (Transaction tran = new Transaction(doc, "AutoDim for Column"))
            {
                tran.Start();
                TWallColumnGroupUtil.Do(formData.TWallColumnGroupVM.ModelItem);
                tran.Commit();
            }

            form.Close();
        }

        public static void HandleCancleClick(this FormData data)
        {
            var form = data.Form;
            form.Close();
        }
    }
}
