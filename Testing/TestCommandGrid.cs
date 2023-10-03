using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using System.Linq;
using SingleData;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace HTAddin
{
    [Transaction(TransactionMode.Manual)]
    public class TestCommandGrid : IExternalCommand
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
            ////
            var grids = Util.SelectedGrids(uiDoc);
            XYZ pickedPoint = sel.PickPoint("Chọn một điểm trên màn hình");
            if (grids.Count > 1)
            {
                ReferenceArray RA = new ReferenceArray();
                List<XYZ> points = new List<XYZ>();
                XYZ direction = new XYZ();

                foreach (var grid in grids)
                {
                    Options op = app.Create.NewGeometryOptions();
                    op.ComputeReferences = true;
                    op.IncludeNonVisibleObjects = true;
                    op.View = doc.ActiveView;
                    GeometryElement GE = grid.get_Geometry(op) as GeometryElement;

                    foreach (GeometryObject GO in GE)
                    {
                        if (GO is Line l)
                        {
                            RA.Append(l.Reference);
                            XYZ point0 = l.GetEndPoint(0);
                            XYZ point1 = l.GetEndPoint(1);
                            XYZ targetPoint = new XYZ();
                            if (pickedPoint.DistanceTo(point0) < pickedPoint.DistanceTo(point1))
                            {
                                direction = (point1 - point0).Normalize();
                                points.Add(point0);
                            }
                            else
                            {
                                direction = (point0 - point1).Normalize();
                                points.Add(point1);
                            }

                            break;
                        }
                    }
                }

                var dimLine = Line.CreateBound(points[0], points[1]);

                Util.ExecuteTransaction(doc, "Testing!", codeToExecute =>
                {
                    var dim = doc.Create.NewDimension(doc.ActiveView, dimLine, RA);

                    var extendDistance = Util.MmToFeet(300);
                    ElementTransformUtils.MoveElement(doc, dim.Id, extendDistance * direction);
                });
            }
            ////
            return Result.Succeeded;
        }
        public class GridSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {
                if (elem is Grid)
                {
                    return true;
                }
                return false;
            }

            public bool AllowReference(Reference reference, XYZ position)
            {
                return false;
            }
        }

    }
}
