using System.Numerics;

public class RotateY : Hittable 
{
    public float Angle { get; }
    public Hittable Object { get; }
    public AABB Bounds { get; }
    public float SinTheta { get; }
    public float CosTheta { get; }

    public RotateY(in Hittable obj, float angle)
    {
        Object = obj;
        Angle = angle;

        float angle_rad = Util.Deg_To_Rad(Angle);
        CosTheta = MathF.Cos(angle_rad);
        SinTheta = MathF.Sin(angle_rad);

        Vector3 mins = new Vector3(float.PositiveInfinity, obj.WorldBound().Minimum.Y, float.PositiveInfinity);
        Vector3 maxs = new Vector3(float.NegativeInfinity, obj.WorldBound().Maximum.Y, float.NegativeInfinity);

        for ( int i = 0; i < 2; ++i )
        {
            for ( int j = 0; j < 2; ++j )
            {
                float x = (1-i)*obj.WorldBound().Minimum.X + i*obj.WorldBound().Maximum.X;
                float z = (1-j)*obj.WorldBound().Minimum.Z + j*obj.WorldBound().Maximum.Z;

                float new_x = x*CosTheta + z*SinTheta;
                float new_z = -x*SinTheta + z*CosTheta;

                mins[0] = Math.Min(mins[0], new_x);
                mins[2] = Math.Min(mins[2], new_z);
                maxs[0] = Math.Max(maxs[0], new_x);
                maxs[2] = Math.Max(maxs[2], new_z);
            }
        }

        Bounds = new AABB(mins, maxs);
    }

    /*
        calculate aabb in world space, used for building bvh

        if hit is triggered in world space aabb, 
        then it is cheaper to move ray into obj space to do hit test

        note: (inverse rot matrix same as transpose of same matrix)
        convert world space ray to obj space w/ transposed rotation matrix
        do hit test with rotated ray
        move hit results back into world space w/ rotation matrix
    */
    public override bool Hit(in Ray r, float t_min, float t_max, ref HitRecord rec)
    {
        Vector3 new_origin = r.Origin;
        Vector3 new_dir = r.Dir;

        // move ray to obj space
        new_origin[0] = r.Origin[0]*CosTheta - r.Origin[2]*SinTheta;
        new_origin[2] = r.Origin[0]*SinTheta + r.Origin[2]*CosTheta;

        new_dir[0] = r.Dir[0]*CosTheta - r.Dir[2]*SinTheta;
        new_dir[2] = r.Dir[0]*SinTheta + r.Dir[2]*CosTheta;

        Ray rotated_r = new Ray(new_origin, new_dir, r.Time);

        if ( !Object.Hit(rotated_r, t_min, t_max, ref rec) )
        {
            return false;
        }

        // move hit results to world space
        Vector3 new_p = rec.P;
        new_p[0] = rec.P[0]*CosTheta + rec.P[2]*SinTheta;
        new_p[2] = -rec.P[0]*SinTheta + rec.P[2]*CosTheta;

        Vector3 new_normal = rec.Normal;
        new_normal[0] = rec.Normal[0]*CosTheta + rec.Normal[2]*SinTheta;
        new_normal[2] = -rec.Normal[0]*SinTheta + rec.Normal[2]*CosTheta;

        rec.P = new_p;
        rec.SetFaceNormal(rotated_r, new_normal);

        return true;
    }

    public override AABB WorldBound(float time0 = 0, float time1 = 1)
    {
        return Bounds;
    }
}