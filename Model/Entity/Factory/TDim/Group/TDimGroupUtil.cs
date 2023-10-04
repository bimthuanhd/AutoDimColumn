using Autodesk.Revit.DB;
using SingleData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAddin
{
    public class TDimGroupUtil
    {
        public static void Do(TDimGroup q)
        {
            var doc = Data.Instance.doc;
            var input = q.Tcolumn.Dict.Input;

            foreach (var detailTDim in q.DetailTDims)
            {
                Dimension dim = null;
                if (input.Selected_DimStyle == null)
                {
                    dim = doc.Create.NewDimension(doc.ActiveView, detailTDim.Line, detailTDim.RA);
                }
                else
                {
                    dim = doc.Create.NewDimension(doc.ActiveView, detailTDim.Line, detailTDim.RA, input.Selected_DimStyle);
                }
                var extendDistance = Util.MmToFeet(300);
                ElementTransformUtils.MoveElement(doc, dim.Id, extendDistance * detailTDim.Direction);
            }

            if (q.Tcolumn.Dict.Input.IsHaveTotalDim)
            {
                foreach (var totalTDim in q.TotalTDims)
                {
                    Dimension dim = null;
                    if (input.Selected_DimStyle == null)
                    {
                        dim = doc.Create.NewDimension(doc.ActiveView, totalTDim.Line, totalTDim.RA);
                    }
                    else
                    {
                        dim = doc.Create.NewDimension(doc.ActiveView, totalTDim.Line, totalTDim.RA, input.Selected_DimStyle);
                    }
                    var extendDistance = Util.MmToFeet(500);
                    ElementTransformUtils.MoveElement(doc, dim.Id, extendDistance * totalTDim.Direction);
                }
            }

        }
        public static List<TDim> GetDetailTDims(TDimGroup q)
        {
            var tDims = new List<TDim>();

            // tDim1
            var tDim1 = new TDim()
            {
                Dict = q,
                Direction = q.Directions[0],
                Face = q.Faces[0],
                TargetPoint = new List<XYZ>()
                {
                    q.Points[0],
                    q.Points[1]
                },
                RA = new ReferenceArray()
            };
            tDim1.RA.Append(q.RA.get_Item(0));
            tDim1.RA.Append(q.RA.get_Item(1));
            if (tDim1.NearestTGrid.Ref != null)
            {
                tDim1.RA.Append(tDim1.NearestTGrid.Ref);
            }

            // tDim2
            var tDim2 = new TDim()
            {
                Dict = q,
                Direction = q.Directions[1],
                Face = q.Faces[1],
                TargetPoint = new List<XYZ>()
                {
                    q.Points[2],
                    q.Points[3]
                },
                RA = new ReferenceArray()
            };
            tDim2.RA.Append(q.RA.get_Item(2));
            tDim2.RA.Append(q.RA.get_Item(3));
            if (tDim2.NearestTGrid.Ref != null)
            {
                tDim2.RA.Append(tDim2.NearestTGrid.Ref);
            }

            tDims.Add(tDim1);
            tDims.Add(tDim2);

            return tDims;
        }

        public static List<TDim> GetTotalTDims(TDimGroup q)
        {
            var tDims = new List<TDim>();

            // tDim1
            var detalTDim1 = q.DetailTDims[0];
            var columnDistance1 = q.Points[0].DistanceTo(q.Points[1]);

            var GridStartPoint1 = detalTDim1.NearestTGrid.Line.GetEndPoint(0);
            var GridEndPoint1 = detalTDim1.NearestTGrid.Line.GetEndPoint(1);
            GridStartPoint1 = new XYZ(GridStartPoint1.X, GridStartPoint1.Y, q.Points[0].Z);
            GridEndPoint1 = new XYZ(GridEndPoint1.X, GridEndPoint1.Y, q.Points[0].Z);
            var newLine1 = Line.CreateBound(GridStartPoint1, GridEndPoint1);

            var p0_GridDistance1 = newLine1.Distance(q.Points[0]);
            var p1_GridDistance1 = newLine1.Distance(q.Points[1]);
            var maxP_GridDistance1 = Math.Max(p0_GridDistance1, p1_GridDistance1);

            if (maxP_GridDistance1 < columnDistance1)
            {
                var tDim1 = new TDim()
                {
                    Dict = q,
                    Direction = q.Directions[0],
                    Face = q.Faces[0],
                    TargetPoint = new List<XYZ>()
                {
                     q.Points[0],
                    q.Points[1]
                },
                    RA = new ReferenceArray()
                };
                tDim1.RA.Append(q.RA.get_Item(0));
                tDim1.RA.Append(q.RA.get_Item(1));

                tDims.Add(tDim1);
            }

            // tDim2
            var detalTDim2 = q.DetailTDims[1];
            var columnDistance2 = q.Points[2].DistanceTo(q.Points[3]);

            var GridStartPoint2 = detalTDim2.NearestTGrid.Line.GetEndPoint(0);
            var GridEndPoint2 = detalTDim2.NearestTGrid.Line.GetEndPoint(1);
            GridStartPoint2 = new XYZ(GridStartPoint2.X, GridStartPoint2.Y, q.Points[3].Z);
            GridEndPoint2 = new XYZ(GridEndPoint2.X, GridEndPoint2.Y, q.Points[3].Z);
            var newLine2 = Line.CreateBound(GridStartPoint2, GridEndPoint2);

            var p0_GridDistance2 = newLine2.Distance(q.Points[2]);
            var p1_GridDistance2 = newLine2.Distance(q.Points[3]);
            var maxP_GridDistance2 = Math.Max(p0_GridDistance2, p1_GridDistance2);

            if (maxP_GridDistance2 < columnDistance2)
            {
                var tDim2 = new TDim()
                {
                    Dict = q,
                    Direction = q.Directions[1],
                    Face = q.Faces[1],
                    TargetPoint = new List<XYZ>()
                {
                     q.Points[2],
                    q.Points[3]
                },
                    RA = new ReferenceArray()
                };
                tDim2.RA.Append(q.RA.get_Item(2));
                tDim2.RA.Append(q.RA.get_Item(3));

                tDims.Add(tDim2);
            }

            return tDims;
        }

    }
}
