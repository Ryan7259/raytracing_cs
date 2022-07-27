using System.Numerics;

public class Box : Hittable
{
    public Vector3 Minimum { get; }
    public Vector3 Maximum { get; }
    public List<Hittable> Sides { get; }

    public Box(Vector3 p0, Vector3 p1, Material mat) 
    {
        Minimum = p0;
        Maximum = p1;

        Sides = new List<Hittable>() {
            new XZRect(Minimum.X, Maximum.X, Minimum.Z, Maximum.Z, Minimum.Y, mat),
            new XZRect(Minimum.X, Maximum.X, Minimum.Z, Maximum.Z, Maximum.Y, mat),

            new XYRect(Minimum.X, Maximum.X, Minimum.Y, Maximum.Y, Minimum.Z, mat),
            new XYRect(Minimum.X, Maximum.X, Minimum.Y, Maximum.Y, Maximum.Z, mat),

            new YZRect(Minimum.Y, Maximum.Y, Minimum.Z, Maximum.Z, Minimum.X, mat),
            new YZRect(Minimum.Y, Maximum.Y, Minimum.Z, Maximum.Z, Maximum.X, mat)
        };
    }

    public override bool Hit(in Ray r, float t_min, float t_max, ref HitRecord rec)
    {
        float t_closest = t_max;
        HitRecord temp_rec = new HitRecord();
        bool hit_side = false;

        foreach ( Hittable side in Sides )
        {
            if ( side.Hit(r, t_min, t_closest, ref temp_rec) )
            {
                hit_side = true;
                t_closest = temp_rec.T;
                rec = temp_rec;
            }
        }

        return hit_side;
    }

    public override AABB WorldBound(float time0 = 0, float time1 = 1)
    {
        return new AABB(Minimum, Maximum);
    }
}