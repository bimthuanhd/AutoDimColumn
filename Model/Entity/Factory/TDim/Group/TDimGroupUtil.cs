using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAddin
{
    public class TDimGroupUtil
    {
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
            var columnDistance = q.Points[0].DistanceTo(q.Points[1]);
            var p0_GridDistance = q.DetailTDims[0].NearestTGrid.Line.Distance(q.Points[0]);
            var p1_GridDistance = q.DetailTDims[0].NearestTGrid.Line.Distance(q.Points[1]);
            var maxP_GridDistance = Math.Max(p0_GridDistance, p1_GridDistance);
            if (maxP_GridDistance < columnDistance)
            {
                var tDIm1 = new TDim()
                {

                };

            }

            return tDims;
        }

    }
}
