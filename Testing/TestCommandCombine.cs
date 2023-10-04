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

            ////
            var selectedGrids = Util.GetAllGrids(doc);
            List<FamilyInstance> columnins = Util.SelectElementsbyType(uiDoc, BuiltInCategory.OST_StructuralColumns);
            foreach (var columnin in columnins)
            {
                var column = doc.GetElement(columnin.Id);
                XYZ locationPoint = (column.Location as LocationPoint).Point;

                Options op = app.Create.NewGeometryOptions();
                op.ComputeReferences = true;
                op.IncludeNonVisibleObjects = true;

                double tolerance = 1e-6;
                FaceArray RAnotBasicZ = new FaceArray();
                ReferenceArray filteredRA = new ReferenceArray();

                List<XYZ> filteredPoints = new List<XYZ>();
                List<XYZ> directions = new List<XYZ>();
                List<Face> faces = new List<Face>();

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
                                        faces.Add(facei);

                                        addedPlannarFaces.Add(plannarFacei);
                                        addedPlannarFaces.Add(plannarFacej);
                                    }
                                }
                            }
                        }
                        break;
                    }
                }

                var face1 = faces[0];
                var face2 = faces[1];
                Line lineGr1 = null;
                Line lineGr2 = null;
                Grid grid1 = null;
                Grid grid2 = null;
                Reference refGr1 = null;
                Reference refGr2 = null;

                // find nearestGrid
                foreach (var grid in selectedGrids)
                {
                    Options opgr = app.Create.NewGeometryOptions();
                    op.ComputeReferences = true;
                    op.IncludeNonVisibleObjects = true;
                    op.View = doc.ActiveView;
                    GeometryElement GEgr = grid.get_Geometry(op) as GeometryElement;
                    XYZ directionGr = null;
                    Line lineGr = null;
                    Reference refGr = null;

                    foreach (GeometryObject GO in GEgr)
                    {
                        if (GO is Line l)
                        {
                            lineGr = l;
                            directionGr = l.Direction;
                            refGr = l.Reference;
                            break;
                        }
                    }

                    var pf1 = face1 as PlanarFace;
                    var pf2 = face2 as PlanarFace;

                    // tim grid gan nhat cot face1
                    if (pf1.ComputeNormal(new UV()).IsAlmostEqualTo(directionGr, tolerance) ||
                        pf1.ComputeNormal(new UV()).IsAlmostEqualTo(-directionGr, tolerance))
                    {
                        if (grid1 == null)
                        {
                            grid1 = grid;
                            lineGr1 = lineGr;
                            refGr1 = refGr;
                        }
                        else
                        {
                            var currentDis = CalculateDistanceToLine(locationPoint, lineGr);
                            var dis1 = CalculateDistanceToLine(locationPoint, lineGr1);
                            if (dis1 > currentDis)
                            {
                                grid1 = grid;
                                lineGr1 = lineGr;
                                refGr1 = refGr;
                            }
                        }
                    }

                    // tim grid gan nhat cot face2
                    if (pf2.ComputeNormal(new UV()).IsAlmostEqualTo(directionGr, tolerance) ||
                        pf2.ComputeNormal(new UV()).IsAlmostEqualTo(-directionGr, tolerance))
                    {
                        if (grid2 == null)
                        {
                            grid2 = grid;
                            lineGr2 = lineGr;
                            refGr2 = refGr;
                        }
                        else
                        {
                            var currentDis = CalculateDistanceToLine(locationPoint, lineGr);
                            var dis2 = CalculateDistanceToLine(locationPoint, lineGr2);
                            if (dis2 > currentDis)
                            {
                                grid2 = grid;
                                lineGr2 = lineGr;
                                refGr2 = refGr;
                            }
                        }
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

                if (refGr1!=null)
                {
                    RA2.Append(refGr1);
                }

                if (refGr2 != null)
                {
                    RA1.Append(refGr2);
                }

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
        public static double CalculateDistanceToLine(XYZ point, Line line)
        {
            XYZ lineStart = line.GetEndPoint(0);
            XYZ lineEnd = line.GetEndPoint(1);
            XYZ vector1 = point - lineStart;
            XYZ vector2 = lineEnd - lineStart;
            double distance = vector1.CrossProduct(vector2).GetLength() / vector2.GetLength();
            return distance;
        }

    }
   
}
