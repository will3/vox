using UnityEngine;
using System.Collections;

public class Raycast
{
    public class RaycastResult {
        public Vector3 HitPos;
        public Vector3 HitNormal;
    }

    public static RaycastResult Trace(Chunks chunks, Vector3 pos, Vector3 dir, int max_d, bool ignoreFirst = true)
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

        float t = 0.0f;
        float ix = Mathf.Floor(px);
        float iy = Mathf.Floor(py);
        float iz = Mathf.Floor(pz);

        float stepx = (dx > 0) ? 1 : -1;
        float stepy = (dy > 0) ? 1 : -1;
        float stepz = (dz > 0) ? 1 : -1;

        // dx,dy,dz are already normalized
        float txDelta = Mathf.Abs(1 / dx);
        float tyDelta = Mathf.Abs(1 / dy);
        float tzDelta = Mathf.Abs(1 / dz);

        float xdist = (stepx > 0) ? (ix + 1 - px) : (px - ix);
        float ydist = (stepy > 0) ? (iy + 1 - py) : (py - iy);
        float zdist = (stepz > 0) ? (iz + 1 - pz) : (pz - iz);

        // location of nearest voxel boundary, in units of t
        float txMax = txDelta * xdist;
        float tyMax = tyDelta * ydist;
        float tzMax = tzDelta * zdist;

        float steppedIndex = -1;
        bool first = true;

        // main loop along raycast vector
        while (t <= max_d)
        {

            // exit check
            var b = chunks.Get((int)ix, (int)iy, (int)iz);

            if (b > 0.5f)
            {
                if (ignoreFirst && first) {
                    first = false;
                } else {
                    var result = new RaycastResult();
                    result.HitPos.x = px + t * dx;
                    result.HitPos.y = py + t * dy;
                    result.HitPos.z = pz + t * dz;


                    if (steppedIndex == 0) result.HitNormal.x = -stepx;

                    if (steppedIndex == 1) result.HitNormal.y = -stepy;

                    if (steppedIndex == 2) result.HitNormal.z = -stepz;

                    return result;    
                }
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