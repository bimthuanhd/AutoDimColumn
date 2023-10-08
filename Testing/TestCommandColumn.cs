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
            List<Element> wallels = Util.SelectElementsbyTypes(uiDoc, new List<BuiltInCategory>() { BuiltInCategory.OST_Walls });
            foreach (var wallel in wallels)
            {
                var wall = doc.GetElement(wallel.Id);

                Options op = app.Create.NewGeometryOptions();
                op.ComputeReferences = true;
                op.IncludeNonVisibleObjects = true;

                double tolerance = 1e-6;
                FaceArray RAnotBasicZ = new FaceArray();
                ReferenceArray filteredRA = new ReferenceArray();

                List<XYZ> filteredPoints = new List<XYZ>();
                List<XYZ> directions = new List<XYZ>();

                GeometryElement GE = wall.get_Geometry(op) as GeometryElement;
                foreach (GeometryObject GO in GE)
                {
                    if (GO is Solid solid && solid.Faces.Size == 6)
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

                                        int edgeLoopIndex = 0;
                                        for (int ind = 0; ind < pfi.EdgeLoops.get_Item(0).Size; ind++)
                                        {
                                            var find_EdgeLoopPoint = pfi.EdgeLoops.get_Item(0).get_Item(ind);
                                            Line find_Linei = find_EdgeLoopPoint.AsCurve() as Line;
                                            XYZ find_Pointi = find_Linei.GetEndPoint(0);
                                            XYZ find_Pointi1 = find_Linei.GetEndPoint(1);
                                            if (find_Pointi.Z == find_Pointi1.Z)
                                            {
                                                edgeLoopIndex = ind;
                                                break;
                                            }
                                        }

                                        Edge edgeLoopPointi = pfi.EdgeLoops.get_Item(0).get_Item(edgeLoopIndex);
                                        Line linei = edgeLoopPointi.AsCurve() as Line;
                                        XYZ pointi = linei.GetEndPoint(0);
                                        filteredPoints.Add(pointi);

                                        XYZ pointi1 = linei.GetEndPoint(1);
                                        XYZ direct = (pointi - pointi1).Normalize();
                                        directions.Add(direct);

                                        Edge edgeLoopPointj = pfj.EdgeLoops.get_Item(0).get_Item(edgeLoopIndex);
                                        Line linej = edgeLoopPointj.AsCurve() as Line;
                                        XYZ pointj = new XYZ(linej.GetEndPoint(0).X, linej.GetEndPoint(0).Y, pointi.Z);
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



    }
}
