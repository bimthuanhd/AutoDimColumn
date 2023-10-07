using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SingleData;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAddin
{
    public static class TColumnUtil
    {
        public static int GetColumnIndex(this TColumn q)
        {
            return q.Dict.TColumns.IndexOf(q);
        }

        public static string GetType(TColumn q)
        {
            var type = "Column";
            var categoryName = q.Column.Category.Name;
            switch (categoryName)
            {
                case "Walls":
                    type = "Wall";
                    break;
                case "Structural Columns":
                    type = "Column";
                    break;
                default:
                    break;
            }
            return type;
        }

        public static TDimGroup GetTDimGroup(this TColumn q)
        {
            var tDimGroup = new TDimGroup();
            switch (q.Type)
            {
                case "Column":
                    tDimGroup = q.Get_ColumnTDimGroup();
                    break;
                case "Wall":
                    tDimGroup = q.Get_WallTDimGroup();
                    break;

                default:
                    break;
            }

            return tDimGroup;
        }

        public static TDimGroup Get_ColumnTDimGroup(this TColumn q)
        {
            var app = Data.Instance.app;

            Options op = app.Create.NewGeometryOptions();
            op.ComputeReferences = true;
            op.IncludeNonVisibleObjects = true;

            double tolerance = 1e-6;
            FaceArray notBasicZ_FA = new FaceArray();

            ReferenceArray filtered_RA = new ReferenceArray();
            List<XYZ> filtered_Points = new List<XYZ>();
            List<XYZ> directions = new List<XYZ>();
            List<Face> faces = new List<Face>();

            GeometryElement gE = q.Column.get_Geometry(op) as GeometryElement;
            foreach (GeometryObject gO in gE)
            {
                if (gO is Solid solid)
                {
                    FaceArray fA = solid.Faces;

                    // filter notBasicZ
                    foreach (Face face in fA)
                    {
                        PlanarFace pf = face as PlanarFace;
                        if (!pf.ComputeNormal(new UV()).IsAlmostEqualTo(XYZ.BasisZ, tolerance) &&
                            !pf.ComputeNormal(new UV()).IsAlmostEqualTo(-XYZ.BasisZ, tolerance))
                        {
                            notBasicZ_FA.Append(face);
                        }
                    }

                    // find referenceArray, directions, points, faces
                    HashSet<Tuple<PlanarFace>> addedPlannarFaces = new HashSet<Tuple<PlanarFace>>();
                    for (int i = 0; i < notBasicZ_FA.Size - 1; i++)
                    {
                        for (int j = i + 1; j < notBasicZ_FA.Size; j++)
                        {
                            var facei = notBasicZ_FA.get_Item(i);
                            var facej = notBasicZ_FA.get_Item(j);
                            PlanarFace pfi = facei as PlanarFace;
                            PlanarFace pfj = facej as PlanarFace;
                            if (Math.Abs(facei.ComputeNormal(new UV()).DotProduct(facej.ComputeNormal(new UV()))) > 1 - tolerance)
                            {
                                Tuple<PlanarFace> plannarFacei = new Tuple<PlanarFace>(pfi);
                                Tuple<PlanarFace> plannarFacej = new Tuple<PlanarFace>(pfj);
                                if (!addedPlannarFaces.Contains(plannarFacei) && !addedPlannarFaces.Contains(plannarFacej))
                                {
                                    filtered_RA.Append(pfi.Reference);
                                    filtered_RA.Append(pfj.Reference);

                                    Edge edgeLoopPointi = pfi.EdgeLoops.get_Item(0).get_Item(0);
                                    Line linei = edgeLoopPointi.AsCurve() as Line;
                                    XYZ pointi = linei.GetEndPoint(0);
                                    filtered_Points.Add(pointi);

                                    XYZ pointi1 = linei.GetEndPoint(1);
                                    XYZ direct = (pointi - pointi1).Normalize();
                                    directions.Add(direct);

                                    Edge edgeLoopPointj = pfj.EdgeLoops.get_Item(0).get_Item(0);
                                    Line linej = edgeLoopPointj.AsCurve() as Line;
                                    XYZ pointj = linej.GetEndPoint(0);
                                    filtered_Points.Add(pointj);
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

            var tDimGroup = new TDimGroup()
            {
                Tcolumn = q,
                RA = filtered_RA,
                Points = filtered_Points,
                Directions = directions,
                Faces = faces,
            };

            return tDimGroup;
        }
        public static TDimGroup Get_WallTDimGroup(this TColumn q)
        {
            var app = Data.Instance.app;

            Options op = app.Create.NewGeometryOptions();
            op.ComputeReferences = true;
            op.IncludeNonVisibleObjects = true;

            double tolerance = 1e-6;
            FaceArray notBasicZ_FA = new FaceArray();

            ReferenceArray filtered_RA = new ReferenceArray();
            List<XYZ> filtered_Points = new List<XYZ>();
            List<XYZ> directions = new List<XYZ>();
            List<Face> faces = new List<Face>();

            GeometryElement gE = q.Column.get_Geometry(op) as GeometryElement;
            foreach (GeometryObject gO in gE)
            {
                if (gO is Solid solid && solid.Faces.Size == 6)
                {
                    FaceArray fA = solid.Faces;

                    // filter notBasicZ
                    foreach (Face face in fA)
                    {
                        PlanarFace pf = face as PlanarFace;
                        if (!pf.ComputeNormal(new UV()).IsAlmostEqualTo(XYZ.BasisZ, tolerance) &&
                            !pf.ComputeNormal(new UV()).IsAlmostEqualTo(-XYZ.BasisZ, tolerance))
                        {
                            notBasicZ_FA.Append(face);
                        }
                    }

                    // find referenceArray, directions, points, faces
                    HashSet<Tuple<PlanarFace>> addedPlannarFaces = new HashSet<Tuple<PlanarFace>>();
                    for (int i = 0; i < notBasicZ_FA.Size - 1; i++)
                    {
                        for (int j = i + 1; j < notBasicZ_FA.Size; j++)
                        {
                            var facei = notBasicZ_FA.get_Item(i);
                            var facej = notBasicZ_FA.get_Item(j);
                            PlanarFace pfi = facei as PlanarFace;
                            PlanarFace pfj = facej as PlanarFace;
                            if (Math.Abs(facei.ComputeNormal(new UV()).DotProduct(facej.ComputeNormal(new UV()))) > 1 - tolerance)
                            {
                                Tuple<PlanarFace> plannarFacei = new Tuple<PlanarFace>(pfi);
                                Tuple<PlanarFace> plannarFacej = new Tuple<PlanarFace>(pfj);
                                if (!addedPlannarFaces.Contains(plannarFacei) && !addedPlannarFaces.Contains(plannarFacej))
                                {
                                    filtered_RA.Append(pfi.Reference);
                                    filtered_RA.Append(pfj.Reference);

                                    Edge edgeLoopPointi = pfi.EdgeLoops.get_Item(0).get_Item(0);
                                    Line linei = edgeLoopPointi.AsCurve() as Line;
                                    XYZ pointi = linei.GetEndPoint(0);
                                    filtered_Points.Add(pointi);

                                    XYZ pointi1 = linei.GetEndPoint(1);
                                    XYZ direct = (pointi - pointi1).Normalize();
                                    directions.Add(direct);

                                    Edge edgeLoopPointj = pfj.EdgeLoops.get_Item(0).get_Item(0);
                                    Line linej = edgeLoopPointj.AsCurve() as Line;
                                    XYZ pointj = linej.GetEndPoint(0);
                                    filtered_Points.Add(pointj);
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

            var tDimGroup = new TDimGroup()
            {
                Tcolumn = q,
                RA = filtered_RA,
                Points = filtered_Points,
                Directions = directions,
                Faces = faces,
            };

            return tDimGroup;
        }

    }
}
