using System.Numerics;

public class Translate : Hittable {
    public Hittable Object { get; }
    public Vector3 Offset { get; }
    public AABB Bounds { get; }

    public Translate(in Hittable obj, Vector3 offset)
    {
        Object = obj;
        Offset = offset;
        Bounds = new AABB(Object.WorldBound().Minimum+offset, Object.WorldBound().Maximum+offset);

    }

    /*
        keep original obj at same place,
        move ray by a negated offset so ray is oriented around original obj
        perform hit calc on original obj positions

        return results from original loc for actual loc 
    */
    public override bool Hit(in Ray r, float t_min, float t_max, ref HitRecord rec)
    {
        Ray new_r = new Ray(r.Origin - Offset, r.Dir, r.Time);
        if ( !Object.Hit(new_r, t_min, t_max, ref rec) )
        {
            return false;
        }

        rec.P += Offset;
        rec.SetFaceNormal(new_r, rec.Normal);
        return true;
    }

    public override AABB WorldBound(float time0 = 0, float time1 = 1)
    {
        return Bounds;
    }
}