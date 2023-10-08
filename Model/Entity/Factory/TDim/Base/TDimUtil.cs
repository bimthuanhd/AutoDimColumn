using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using SingleData;

namespace HTAddin
{
    public class TDimUtil
    {
        public static Line GetLine(TDim q)
        {
            Line line = null;
            if (q.TargetPoint.Count > 1)
            {
                line = Line.CreateBound(q.TargetPoint[0], q.TargetPoint[1]);
            }

            return line;
        }
        public static TGrid GetNearestTGrid(TDim q)
        {
            TGrid tGrid = null;

            switch (q.Dict.TWallColumn.Type)
            {
                case "Column":
                    tGrid = Get_ColumnNearestTGrid(q);
                    break;
                case "Wall":
                    tGrid = Get_WallNearestTGrid(q);
                    break;
                default:
                    break;
            }
            return tGrid;
        }
        public static TGrid Get_ColumnNearestTGrid(TDim q)
        {
            var app = Data.Instance.app;
            var doc = Data.Instance.doc;
            XYZ locationPoint = (q.Dict.TWallColumn.WallColumn.Location as LocationPoint).Point;
            double tolerance = 1e-6;
            Grid currentGrid = null;
            Reference currentRef = null;
            Line currentLine = null;

            foreach (var grid in q.Dict.TWallColumn.Dict.ProjectGrids)
            {
                Options op = app.Create.NewGeometryOptions();
                op.ComputeReferences = true;
                op.IncludeNonVisibleObjects = true;
                op.View = doc.ActiveView;
                GeometryElement gE = grid.get_Geometry(op) as GeometryElement;
                XYZ directionGr = null;
                Line lineGr = null;
                Reference refGr = null;

                foreach (GeometryObject gO in gE)
                {
                    if (gO is Line l)
                    {
                        lineGr = l;
                        directionGr = l.Direction;
                        refGr = l.Reference;
                        break;
                    }
                }

                var pf = q.Face as PlanarFace;

                // tim grid gan nhat cot
                if (Util.IsPerpendicularToDirection(pf, directionGr, tolerance) ||
                    Util.IsPerpendicularToDirection(pf, -directionGr, tolerance))
                {
                    if (currentGrid == null)
                    {
                        currentGrid = grid;
                        currentRef = refGr;
                        currentLine = lineGr;
                    }
                    else
                    {
                        var currentDis = Util.CalculateDistanceToLine(locationPoint, currentLine);
                        var dis = Util.CalculateDistanceToLine(locationPoint, lineGr);
                        if (dis < currentDis)
                        {
                            currentGrid = grid;
                            currentLine = lineGr;
                            currentRef = refGr;
                        }
                    }
                }
            }

            var tGrid = new TGrid()
            {
                Grid = currentGrid,
                Line = currentLine,
                Ref = currentRef,
            };

            return tGrid;
        }
        public static TGrid Get_WallNearestTGrid(TDim q)
        {
            var app = Data.Instance.app;
            var doc = Data.Instance.doc;
            XYZ locationPoint = Get_FaceZCenterPoint(q);

            double tolerance = 1e-6;
            Grid currentGrid = null;
            Reference currentRef = null;
            Line currentLine = null;

            foreach (var grid in q.Dict.TWallColumn.Dict.ProjectGrids)
            {
                Options op = app.Create.NewGeometryOptions();
                op.ComputeReferences = true;
                op.IncludeNonVisibleObjects = true;
                op.View = doc.ActiveView;
                GeometryElement gE = grid.get_Geometry(op) as GeometryElement;
                XYZ directionGr = null;
                Line lineGr = null;
                Reference refGr = null;

                foreach (GeometryObject gO in gE)
                {
                    if (gO is Line l)
                    {
                        lineGr = l;
                        directionGr = l.Direction;
                        refGr = l.Reference;
                        break;
                    }
                }

                var pf = q.Face as PlanarFace;

                // tim grid gan nhat cot
                if (Util.IsPerpendicularToDirection(pf, directionGr, tolerance) ||
                    Util.IsPerpendicularToDirection(pf, -directionGr, tolerance))
                {
                    if (currentGrid == null)
                    {
                        currentGrid = grid;
                        currentRef = refGr;
                        currentLine = lineGr;
                    }
                    else
                    {
                        var currentDis = Util.CalculateDistanceToLine(locationPoint, currentLine);
                        var dis = Util.CalculateDistanceToLine(locationPoint, lineGr);
                        if (dis < currentDis)
                        {
                            currentGrid = grid;
                            currentLine = lineGr;
                            currentRef = refGr;
                        }
                    }
                }
            }

            var tGrid = new TGrid()
            {
                Grid = currentGrid,
                Line = currentLine,
                Ref = currentRef,
            };

            return tGrid;
        }

        public static XYZ Get_FaceZCenterPoint(TDim q)
        {
            XYZ centerPoint = new XYZ();

            var wall = q.Dict.TWallColumn.WallColumn;
            Options op = Data.Instance.app.Create.NewGeometryOptions();
            op.ComputeReferences = true;
            op.IncludeNonVisibleObjects = true;

            double tolerance = 1e-6;
            Face firstFaceBasicZ = null;


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
                        if (pf.ComputeNormal(new UV()).IsAlmostEqualTo(XYZ.BasisZ, tolerance) ||
                            pf.ComputeNormal(new UV()).IsAlmostEqualTo(-XYZ.BasisZ, tolerance))
                        {
                            firstFaceBasicZ = face;
                            break;
                        }
                    }
                    break;
                }
            }

            if (firstFaceBasicZ != null)
            {
                var pf = firstFaceBasicZ as PlanarFace;
                Edge edgeLoopPoint0 = pf.EdgeLoops.get_Item(0).get_Item(0);
                Edge edgeLoopPoint1 = pf.EdgeLoops.get_Item(0).get_Item(1);
                XYZ point0 = (edgeLoopPoint0.AsCurve() as Line).GetEndPoint(0);
                XYZ point1 = (edgeLoopPoint1.AsCurve() as Line).GetEndPoint(0);

                centerPoint = (point0 + point1) / 2;
            }

            return centerPoint;
        }
    }
}
