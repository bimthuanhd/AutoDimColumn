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
        public static void Do(TDim q)
        {
            var doc = Data.Instance.doc;
            var dim = doc.Create.NewDimension(doc.ActiveView, q.Line, q.RA);
            var extendDistance = Util.MmToFeet(300);
            ElementTransformUtils.MoveElement(doc, dim.Id, extendDistance * q.Direction);
        }
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
            var app = Data.Instance.app;
            var doc = Data.Instance.doc;
            XYZ locationPoint = (q.Dict.Tcolumn.Column.Location as LocationPoint).Point;
            double tolerance = 1e-6;
            Grid currentGrid = null;
            Reference currentRef = null;
            Line currentLine = null;

            foreach (var grid in q.Dict.Tcolumn.Dict.ProjectGrids)
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
    }
}
