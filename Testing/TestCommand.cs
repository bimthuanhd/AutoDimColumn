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
    public class TestCommand : IExternalCommand
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

            Reference columnRef = sel.PickObject(ObjectType.Element, new ColumnSelectionFilter(), "Chọn cột");

            var column = doc.GetElement(columnRef.ElementId) as FamilyInstance;
            //ElementId symbolId = column.Symbol.Id;
            //FamilySymbol symbol = doc.GetElement(symbolId) as FamilySymbol;

            //Options options = new Options();
            //GeometryElement geometryElement = symbol.get_Geometry(options);

            //Reference leftFaceReference = null;
            //Reference rightFaceReference = null;

            //foreach (GeometryObject obj in geometryElement)
            //{
            //    if (obj is Solid solid)
            //    {
            //        FaceArray faces = solid.Faces;

            //        foreach (Face face in faces)
            //        {
            //            double tolerance = 0.001; // Khoảng chấp nhận
            //            XYZ basisX = XYZ.BasisX;
            //            var currentRef = face.Reference;

            //            TaskDialog.Show("normal", face.ComputeNormal(new UV()).ToString());

            //            if (face.ComputeNormal(new UV()).IsAlmostEqualTo(basisX, tolerance))
            //            {
            //                leftFaceReference = face.Reference;
            //            }

            //            if (face.ComputeNormal(new UV()).IsAlmostEqualTo(-basisX, tolerance))
            //            {
            //                rightFaceReference = face.Reference;
            //            }
            //        }
            //    }
            //}

            //if (leftFaceReference != null && rightFaceReference != null)
            //{
            //    var leftFaceElement = doc.GetElement(leftFaceReference.ElementId);
            //    var rightFaceElement = doc.GetElement(rightFaceReference.ElementId);

            //    var leftFace = GetPlanarFace(leftFaceElement);
            //    var rightFace = GetPlanarFace(rightFaceElement);

            //    if (leftFace != null && rightFace != null)
            //    {
            //        var leftFaceCenter = GetCentroid(leftFace);
            //        var rightFaceCenter = GetCentroid(rightFace);

            //        var line = Line.CreateBound(leftFaceCenter, rightFaceCenter);

            //        using (Transaction trans = new Transaction(doc, "Tạo kích thước cột"))
            //        {
            //            trans.Start();

            //            ReferenceArray references = new ReferenceArray();
            //            references.Append(leftFaceReference);
            //            references.Append(rightFaceReference);

            //            // Tạo kích thước mới
            //            Dimension dimension = doc.Create.NewDimension(doc.ActiveView, line, references);

            //            trans.Commit();
            //        }
            //    }
            //    else
            //    {
            //        TaskDialog.Show("Lỗi", "Không tìm thấy mặt trái và mặt phải của cột");
            //    }
            //}

            List<Edge> columnReferenceEdges = new List<Edge>();
            Options options = new Options();
            GeometryElement geometryElement = column.get_Geometry(options);

            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Columns);
            FindReferenceTarget targetType = FindReferenceTarget.Face;
            ReferenceIntersector referenceIntersector = new ReferenceIntersector(filter, targetType, activeView);


            return Result.Succeeded;
        }

        public static DimensionType GetDimensionTypeByName(Document doc, string typeName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            Element element = collector.OfClass(typeof(DimensionType)).FirstOrDefault(x => x.Name == typeName);
            return element as DimensionType;
        }
        Dimension CreateNewDimensionAlongLine(Document document, Line line)
        {
            // Use the Start and End points of our line as the references  
            // Line must come from something in Revit, such as a beam
            ReferenceArray references = new ReferenceArray();
            references.Append(line.GetEndPointReference(0));
            references.Append(line.GetEndPointReference(1));

            // create the new dimension
            Dimension dimension = document.Create.NewDimension(document.ActiveView, line, references);
            return dimension;
        }
        private Element GetBeamElement(Document doc)
        {
            // Lấy một dầm từ mô hình Revit
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            Element beam = collector.OfCategory(BuiltInCategory.OST_StructuralFraming)
                                    .OfClass(typeof(FamilyInstance))
                                    .FirstOrDefault();

            return beam;
        }
        public void CreateDimensionOnBeam(FamilyInstance beam)
        {
            Document document = beam.Document;
            View activeView = document.ActiveView;

            LocationCurve location = beam.Location as LocationCurve;
            Line line = location.Curve as Line;

            if (line != null)
            {
                ReferenceArray references = new ReferenceArray();
                references.Append(line.GetEndPointReference(0));
                references.Append(line.GetEndPointReference(1));

                Dimension dimension = document.Create.NewDimension(activeView, line, references); ;

            }
            else
            {
                TaskDialog.Show("revit", "line is null");
            }
        }
        public class ColumnSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {
                return elem is FamilyInstance familyInstance && familyInstance.Category != null &&
                       familyInstance.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns;
            }

            public bool AllowReference(Reference reference, XYZ position)
            {
                return false;
            }
        }
        public class BeamSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {
                return elem is FamilyInstance familyInstance && familyInstance.Category != null &&
                       familyInstance.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming;
            }

            public bool AllowReference(Reference reference, XYZ position)
            {
                return false;
            }
        }
        public List<Face> GetBeamSideFaces(FamilyInstance beam)
        {
            Options options = new Options();
            options.ComputeReferences = true;
            GeometryElement geometryElement = beam.get_Geometry(options);

            List<Face> sideFaces = new List<Face>();

            foreach (GeometryObject geometryObject in geometryElement)
            {
                if (geometryObject is Solid solid)
                {
                    foreach (Face face in solid.Faces)
                    {
                        if (IsBeamSideFace(face, beam))
                        {
                            sideFaces.Add(face);
                        }
                    }
                }
            }

            return sideFaces;
        }

        private bool IsBeamSideFace(Face face, FamilyInstance beam)
        {
            XYZ faceNormal = face.ComputeNormal(new UV());

            // Lấy hướng cột của dầm (thay thế bằng phương thức hoặc mã tương ứng)
            XYZ beamDirection = beam.FacingOrientation;

            double dotProduct = faceNormal.DotProduct(beamDirection);

            if (dotProduct > 0)
            {
                return true;
            }

            return false;
        }

        private PlanarFace GetPlanarFace(Element element)
        {
            Options options = new Options();
            GeometryElement geometryElement = element.get_Geometry(options);

            foreach (GeometryObject geometryObject in geometryElement)
            {
                if (geometryObject is Solid solid)
                {
                    foreach (Face face in solid.Faces)
                    {
                        if (face is PlanarFace planarFace)
                        {
                            return planarFace;
                        }
                    }
                }
            }

            return null;
        }
        private XYZ GetCentroid(PlanarFace face)
        {
            var loop = face.GetEdgesAsCurveLoops().FirstOrDefault();
            if (loop != null)
            {
                var normal = face.FaceNormal;
                var startPoint = loop.FirstOrDefault()?.GetEndPoint(0);
                var endPoint = loop.FirstOrDefault()?.GetEndPoint(1);

                if (startPoint != null && endPoint != null)
                {
                    var centroid = (startPoint + endPoint) * 0.5;
                    return centroid + normal * 1;
                }
            }

            return null;
        }
    }
}
