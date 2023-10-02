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
            ////

            Reference refWall = sel.PickObject(ObjectType.Element, new WallSelectionFilter());
            var w = doc.GetElement(refWall.ElementId);

            LocationCurve LC = w.Location as LocationCurve;
            XYZ Normal = findNormal(LC.Curve);
            XYZ lineDir = LC.Curve.GetEndPoint(1) - LC.Curve.GetEndPoint(0);

            Options op = app.Create.NewGeometryOptions();
            op.ComputeReferences = true;
            op.IncludeNonVisibleObjects = true;

            ReferenceArray RA = new ReferenceArray();

            GeometryElement GE = w.get_Geometry(op) as GeometryElement;
            foreach (GeometryObject GO in GE)
            {
                Solid solid = GO as Solid;
                FaceArray FA = solid.Faces;
                foreach (Face f in FA)
                {
                    PlanarFace pf = f as PlanarFace;
                    if (pf != null && isParallel(lineDir, pf.FaceNormal))
                    {
                        RA.Append(pf.Reference);
                    }
                }
            }

            Line l1 = Line.CreateBound(LC.Curve.GetEndPoint(0), LC.Curve.GetEndPoint(1));
            using (Transaction trans = new Transaction(doc, "ABC"))
            {
                trans.Start();
                doc.Create.NewDimension(doc.ActiveView, l1, RA);
                trans.Commit();
            }

            ////
            return Result.Succeeded;
        }

        private bool isParallel(XYZ lineDir, XYZ faceNormal)
        {
            return faceNormal.CrossProduct(lineDir).IsAlmostEqualTo(new XYZ(0, 0, 0));
        }
        private XYZ findNormal(Curve curve)
        {
            XYZ p = curve.GetEndPoint(0);
            XYZ q = curve.GetEndPoint(1);
            XYZ v = q - p;

            return v.CrossProduct(XYZ.BasisZ).Normalize();
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
        public class WallSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {
                return elem is Wall wall && wall.Category != null &&
                       wall.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls;
            }

            public bool AllowReference(Reference reference, XYZ position)
            {
                return false;
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

        private Reference GetFirstGeometryReference(GeometryElement geometryElement)
        {
            Reference firstReference = null;

            foreach (GeometryObject geometryObject in geometryElement)
            {
                if (geometryObject is GeometryInstance geometryInstance)
                {
                    GeometryElement instanceGeometry = geometryInstance.GetInstanceGeometry();
                    firstReference = GetFirstGeometryReference(instanceGeometry);
                    if (firstReference != null)
                        break;
                }
                else if (geometryObject is Solid solid && solid.Faces.Size > 0)
                {
                    Face firstFace = solid.Faces.get_Item(0);
                    firstReference = firstFace.Reference;
                    break;
                }
            }

            return firstReference;
        }
    }
}
