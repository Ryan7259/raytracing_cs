using System.Numerics;

public class AABB 
{
    public Vector3 Minimum { get; set; }
    public Vector3 Maximum { get; set; }
    /*
        R0 + Rdir*t = P
        t = (P - R0) / Rdir
    */

    public AABB()
    {
        Minimum = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        Maximum = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
    }
    public AABB(Vector3 a, Vector3 b)
    {
        Minimum = a;
        Maximum = b;
    }

    public float SurfaceArea()
    {
        return 2.0f * (Maximum.X-Minimum.X + Maximum.Y-Minimum.Y + Maximum.Z-Minimum.Z);
    }
    public Vector3 Diagnol()
    {
        return Maximum - Minimum;
    }
    public int MaximumExtent()
    {
        Vector3 d = Diagnol();
        if ( d.X > d.Y && d.X > d.Z )
        {
            return 0;
        }
        else if ( d.Y > d.Z )
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

    public bool hit(in Ray r, float tmin, float tmax)
    {
        // solve for t with ray r for 1 axis
        Vector3 t0s = (Minimum - r.Origin) * r.InvDirs;
        Vector3 t1s = (Maximum - r.Origin) * r.InvDirs;

        // for all 6 t vals, sort them into tmins and tmaxs
        Vector3 tmins = Vector3.Min(t0s, t1s);
        Vector3 tmaxs = Vector3.Max(t0s, t1s);

        // get max of min and min of max for overlapping intervals
        // since only overlaps of all axes means intersection
        tmin = Math.Max(tmin, Math.Max(tmins[0], Math.Max(tmins[1], tmins[2])));
        tmax = Math.Min(tmax, Math.Min(tmaxs[0], Math.Min(tmaxs[1], tmaxs[2])));

        // if any axis doesn't overlap at all, it doesn't intersect
        // equals is for grazing case where corner of 2 axes are hit
        return tmax > tmin;
    }

    public static AABB Union(in AABB a, in AABB b)
    {
        return new AABB(
            new Vector3(
                Math.Min(a.Minimum.X, b.Minimum.X), 
                Math.Min(a.Minimum.Y, b.Minimum.Y), 
                Math.Min(a.Minimum.Z, b.Minimum.Z)),
            new Vector3(
                Math.Max(a.Maximum.X, b.Maximum.X), 
                Math.Max(a.Maximum.Y, b.Maximum.Y), 
                Math.Max(a.Maximum.Z, b.Maximum.Z))
        );
    }
    public static AABB Union(in AABB a, in Vector3 b)
    {
        return new AABB(
            new Vector3(
                Math.Min(a.Minimum.X, b.X), 
                Math.Min(a.Minimum.Y, b.Y), 
                Math.Min(a.Minimum.Z, b.Z)),
            new Vector3(
                Math.Max(a.Maximum.X, b.X), 
                Math.Max(a.Maximum.Y, b.Y), 
                Math.Max(a.Maximum.Z, b.Z))
        );
    }

    /*
    public Vector3 Offset(in Vector3 a)
    {
        Vector3 o = a - Minimum;
        if ( Maximum.X > Minimum.X ) o /= (Maximum.X-Minimum.X);
        if ( Maximum.Y > Minimum.Y ) o /= (Maximum.Y-Minimum.Y);
        if ( Maximum.Z > Minimum.Z ) o /= (Maximum.Z-Minimum.Z);
        return o;
    }
    */
}