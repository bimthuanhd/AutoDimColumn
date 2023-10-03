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
    public class TestCommandColumn : IExternalCommand
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
            List<FamilyInstance> columnins = Util.SelectElementsbyType(uiDoc, BuiltInCategory.OST_StructuralColumns);
            foreach (var columnin in columnins)
            {
                var column = doc.GetElement(columnin.Id);

                Options op = app.Create.NewGeometryOptions();
                op.ComputeReferences = true;
                op.IncludeNonVisibleObjects = true;

                double tolerance = 1e-6;
                FaceArray RAnotBasicZ = new FaceArray();
                ReferenceArray filteredRA = new ReferenceArray();

                List<XYZ> filteredPoints = new List<XYZ>();
                List<XYZ> directions = new List<XYZ>();

                GeometryElement GE = column.get_Geometry(op) as GeometryElement;
                foreach (GeometryObject GO in GE)
                {
                    if (GO is Solid solid)
                    {
                        FaceArray FA = solid.Faces;

                        // filter //basicZ
                        foreach (Face face in FA)
                        {
                            PlanarFace pf = face as PlanarFace;
                            if (!pf.ComputeNormal(new UV()).IsAlmostEqualTo(XYZ.BasisZ, tolerance) &&
                                !pf.ComputeNormal(new UV()).IsAlmostEqualTo(-XYZ.BasisZ, tolerance))
                            {
                                RAnotBasicZ.Append(face);
                            }
                        }

                        HashSet<Tuple<PlanarFace>> addedPlannarFaces = new HashSet<Tuple<PlanarFace>>();

                        for (int i = 0; i < RAnotBasicZ.Size - 1; i++)
                        {
                            for (int j = i + 1; j < RAnotBasicZ.Size; j++)
                            {
                                var facei = RAnotBasicZ.get_Item(i);
                                var facej = RAnotBasicZ.get_Item(j);
                                PlanarFace pfi = facei as PlanarFace;
                                PlanarFace pfj = facej as PlanarFace;
                                if (Math.Abs(facei.ComputeNormal(new UV()).DotProduct(facej.ComputeNormal(new UV()))) > 1 - tolerance)
                                {
                                    Tuple<PlanarFace> plannarFacei = new Tuple<PlanarFace>(pfi);
                                    Tuple<PlanarFace> plannarFacej = new Tuple<PlanarFace>(pfj);
                                    if (!addedPlannarFaces.Contains(plannarFacei) && !addedPlannarFaces.Contains(plannarFacej))
                                    {
                                        filteredRA.Append(pfi.Reference);
                                        filteredRA.Append(pfj.Reference);

                                        Edge edgeLoopPointi = pfi.EdgeLoops.get_Item(0).get_Item(0);
                                        Line linei = edgeLoopPointi.AsCurve() as Line;
                                        XYZ pointi = linei.GetEndPoint(0);
                                        filteredPoints.Add(pointi);

                                        XYZ pointi1 = linei.GetEndPoint(1);
                                        XYZ direct = (pointi - pointi1).Normalize();
                                        directions.Add(direct);

                                        Edge edgeLoopPointj = pfj.EdgeLoops.get_Item(0).get_Item(0);
                                        Line linej = edgeLoopPointj.AsCurve() as Line;
                                        XYZ pointj = linej.GetEndPoint(0);
                                        filteredPoints.Add(pointj);

                                        addedPlannarFaces.Add(plannarFacei);
                                        addedPlannarFaces.Add(plannarFacej);
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
                var point1 = filteredPoints[0];
                var point2 = filteredPoints[1];
                var point3 = filteredPoints[2];
                var point4 = filteredPoints[3];

                Line l1 = Line.CreateBound(point1, point2);
                Line l2 = Line.CreateBound(point3, point4);

                ReferenceArray RA1 = new ReferenceArray();
                ReferenceArray RA2 = new ReferenceArray();
                RA1.Append(filteredRA.get_Item(0));
                RA1.Append(filteredRA.get_Item(1));
                RA2.Append(filteredRA.get_Item(2));
                RA2.Append(filteredRA.get_Item(3));
                Util.ExecuteTransaction(doc, "Testing!", codeToExecute =>
                 {
                     var dim1 = doc.Create.NewDimension(doc.ActiveView, l1, RA1);
                     var dim2 = doc.Create.NewDimension(doc.ActiveView, l2, RA2);

                     var extendDistance = Util.MmToFeet(300);
                     ElementTransformUtils.MoveElement(doc, dim1.Id, extendDistance * directions[0]);
                     ElementTransformUtils.MoveElement(doc, dim2.Id, extendDistance * directions[1]);

                 });
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
