using System.Numerics;

public class MovingSphere : Hittable 
{
    public float Radius { get; set; }
    public Vector3 Center0 { get; set; }
    public Vector3 Center1 { get; set; }
    public float Time0 { get; set; }
    public float Time1 { get; set; }
    public Material Mat { get; set; }

    public MovingSphere(Vector3 center0, Vector3 center1, float time0, float time1, float radius, Material mat)
    {
        Center0 = center0;
        Center1 = center1;
        Time0 = time0;
        Time1 = time1;
        Radius = radius;
        Mat = mat;
    }

    public override bool hit(in Ray r, float t_min, float t_max, ref HitRecord rec)
    {
        // geometric
        Vector3 L = GetCenter(r.Time) - r.Origin;
        float tc = Vector3.Dot(L, r.Dir);
        if ( tc < 0.0 ) return false; // ray is not in same direction

        float d2 = L.LengthSquared() - tc*tc;
        float radius2 = Radius*Radius;
        if ( d2 > radius2 ) return false; // ray is not within sphere, its dist from center is farther than radius

        float x = (float)Math.Sqrt(radius2 - d2);

        float root = tc - x;
        if ( root < t_min || root > t_max)
        {
            root = tc + x;
            if ( root < t_min || root > t_max )
            {
                return false; // either intersects out of range, or behind another sphere
            }
        }

        rec.p = r.at(root); // record nearest vector intersection point
        rec.set_face_normal(r, (rec.p - GetCenter(r.Time)) / Radius ); // divide by radius b/c pt of intersection to center is just radius OR .unit_vector() to record outward normal
        rec.t = root; // record t value to intersection point from ray origin
        rec.mat = Mat; // set material of hit object

        return true;
    }

    // get the ratio of curTime/totalTime of totalDist and add it to start center
    // if time goes over, we just keep scaling center
    public Vector3 GetCenter(float time)
    {
        return Center0 + ((time - Time0) / (Time1 - Time0)) * (Center1 - Center0);
    }

    public override AABB WorldBound(float time0, float time1)
    {
        AABB box_t0 = new AABB(
            GetCenter(time0) - new Vector3(Radius, Radius, Radius), 
            GetCenter(time0) + new Vector3(Radius, Radius, Radius)
        );
        AABB box_t1 = new AABB(
            GetCenter(time1) - new Vector3(Radius, Radius, Radius),
            GetCenter(time1) + new Vector3(Radius, Radius, Radius)
        );

        return AABB.Union(box_t0, box_t1);
    }
}