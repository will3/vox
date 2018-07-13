using UnityEngine;
using System.Collections;

public class Raycast
{
    class RaycastResult {
        public Vector3 HitPos;
        public Vector3 HitNormal;
    }

    RaycastResult TraceRay(Chunks chunks, Vector3 pos, Vector3 dir, int max_d)
    {
        float px = pos.x;
        float py = pos.y;
        float pz = pos.z;

        float dx = dir.x;
        float dy = dir.y;
        float dz = dir.z;

        // consider raycast vector to be parametrized by t
        //   vec = [px,py,pz] + t * [dx,dy,dz]

        // algo below is as described by this paper:
        // http://www.cse.chalmers.se/edu/year/2010/course/TDA361/grid.pdf

        var t = 0.0f;
        var ix = Mathf.FloorToInt(px) | 0;
        var iy = Mathf.FloorToInt(py) | 0;
        var iz = Mathf.FloorToInt(pz) | 0;

        var stepx = (dx > 0) ? 1 : -1;
        var stepy = (dy > 0) ? 1 : -1;
        var stepz = (dz > 0) ? 1 : -1;

        // dx,dy,dz are already normalized
        var txDelta = Mathf.Abs(1 / dx);
        var tyDelta = Mathf.Abs(1 / dy);
        var tzDelta = Mathf.Abs(1 / dz);

        var xdist = (stepx > 0) ? (ix + 1 - px) : (px - ix);
        var ydist = (stepy > 0) ? (iy + 1 - py) : (py - iy);
        var zdist = (stepz > 0) ? (iz + 1 - pz) : (pz - iz);

        // location of nearest voxel boundary, in units of t
        var txMax = txDelta * xdist;
        var tyMax = tyDelta * ydist;
        var tzMax = tzDelta * zdist;

        var steppedIndex = -1;

        // main loop along raycast vector
        while (t <= max_d)
        {

            // exit check
            var b = chunks.Get(ix, iy, iz);

            if (b > 0.5f)
            {
                var result = new RaycastResult();
                result.HitPos.x = px + t * dx;
                result.HitPos.y = py + t * dy;
                result.HitPos.z = pz + t * dz;


                if (steppedIndex == 0) result.HitNormal.x = -stepx;

                if (steppedIndex == 1) result.HitNormal.y = -stepy;

                if (steppedIndex == 2) result.HitNormal.z = -stepz;

                return result;
            }

            // advance t to next nearest voxel boundary
            if (txMax < tyMax)
            {
                if (txMax < tzMax)
                {
                    ix += stepx;

                    t = txMax;

                    txMax += txDelta;

                    steppedIndex = 0;

                }
                else
                {
                    iz += stepz;

                    t = tzMax;

                    tzMax += tzDelta;

                    steppedIndex = 2;

                }
            }
            else
            {
                if (tyMax < tzMax)
                {
                    iy += stepy;

                    t = tyMax;

                    tyMax += tyDelta;

                    steppedIndex = 1;

                }
                else
                {
                    iz += stepz;

                    t = tzMax;

                    tzMax += tzDelta;

                    steppedIndex = 2;

                }
            }
        }

        return null;
    }


    //// conform inputs

    //function traceRay(getVoxel, origin, direction, max_d, hit_pos, hit_norm)
    //{
    //    var px = +origin[0]
    //        , py = +origin[1]
    //        , pz = +origin[2]
    //        , dx = +direction[0]
    //        , dy = +direction[1]
    //        , dz = +direction[2]
    //        , ds = Math.sqrt(dx * dx + dy * dy + dz * dz)
    

    //if (ds === 0)
    //    {
    //        throw new Error("Can't raycast along a zero vector")

    //}

    //    dx /= ds
    
    //dy /= ds
    
    //dz /= ds
    
    //if (typeof(max_d) === "undefined")
    //    {
    //        max_d = 64.0

    //}
    //    else
    //    {
    //        max_d = +max_d
  
    //}
    //    return traceRay_impl(getVoxel, px, py, pz, dx, dy, dz, max_d, hit_pos, hit_norm)
    //}
}