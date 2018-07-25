using System.Collections.Generic;
using UnityEngine;

namespace FarmVox
{
    class HexPattern
    {
        public static List<Vector3> GetFormation(Vector3 around, int count) {
            var formation = new List<Vector3>();
            var formationCount = 40;
            var ring = 0;

            while (formation.Count < formationCount)
            {
                var coords = GetCoordsInRing(ring);
                foreach (var coord in coords)
                {
                    var down = new Ray(around + coord + new Vector3(0, 100, 0), Vector3.down);
                    RaycastHit downResult;
                    if (Physics.Raycast(down, out downResult))
                    {
                        formation.Add(downResult.point);
                        if (formation.Count == formationCount)
                        {
                            break;
                        }
                    }
                    else
                    {
                        Debug.Log("no result for " + coord.ToString());
                    }
                }

                if (formation.Count == formationCount)
                {
                    break;
                }
                ring += 1;
            }

            return formation;
        }

        public static List<Vector3> GetCoordsInRing(int ring) {
            var coords = new List<Vector3>();

            if (ring == 0)
            {
                coords.Add(new Vector3(0, 0, 0));
                return coords;
            }

            var current = new Vector3(0, 0, ring);
            coords.Add(current);

            var sin = Mathf.Sin(60.0f / 180.0f * Mathf.PI);
            var cos = Mathf.Cos(60.0f / 180.0f * Mathf.PI);

            for (var i = 0; i < 6; i++) {
                for (var j = 0; j < ring; j++) {
                    if (j == 0) {
                        current += new Vector3(sin, 0, cos);
                    } else if (j == 1) {
                        current += new Vector3(0, 0, 1);
                    } else if (j == 2) {
                        current += new Vector3(-sin, 0, cos);
                    } else if (j == 3) {
                        current += new Vector3(-sin, 0, -cos);
                    } else if (j == 4) {
                        current += new Vector3(0, 0, -1);
                    } else if (j == 5) {
                        if (i == 5 && j == ring - 1) {
                            continue;
                        }
                        current += new Vector3(sin, 0, -cos);
                    }

                    coords.Add(current);
                }
            }

            return coords;
        }
    }
}