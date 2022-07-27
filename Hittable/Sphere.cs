using System.Numerics;

public class Sphere : Hittable {
    public float Radius { get; set; }
    public Vector3 Center { get; set; }
    public Material Mat { get; set; }

    public Sphere() 
    {
        Radius = 0f;
        Center = new Vector3();
        Mat = new Lambertian(new Vector3());
    }
    public Sphere(Vector3 center, float radius, Material mat)
    {
        Center = center;
        Radius = radius;
        Mat = mat;
    }

    public override bool Hit(in Ray r, float t_min, float t_max, ref HitRecord rec)
    {
        // geometric
        Vector3 L = Center - r.Origin;
        float tc = Vector3.Dot(L, r.Dir);
        if ( tc < 0 ) return false; // ray is not in same direction

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

        rec.P = r.at(root); // record nearest vector intersection point
        rec.SetFaceNormal(r, (rec.P - Center) / Radius ); // divide by radius b/c pt of intersection to center is just radius OR .unit_vector() to record outward normal
        rec.T = root; // record t value to intersection point from ray origin
        rec.Mat = Mat; // set material of hit object
        (rec.U, rec.V) = GetSphereUV(rec.Normal); // get uv coordinates of point on sphere

        return true;
        /*
        // analytic/quadratic
        Vec3 oc = r.Origin - Center; // NOTE: order is different from geometric method
        float a =  r.Dir.length_squared();
        float half_b = oc.dot(r.Dir);
        float c = oc.length_squared() - Radius*Radius;

        float discriminant = half_b*half_b - a*c;
        if ( discriminant < 0.0 ) return false;

        float sqrtd = Math.Sqrt(discriminant);
        float root = (-half_b - sqrtd) / a;

        if ( root < t_min || t_max < root )
        {
            root = (-half_b + sqrtd) / a;
            if ( root < t_min || t_max < root )
            {
                return false;
            }
        }

        rec.p = r.at(root);
        rec.set_face_normal(r, (rec.p - Center) / Radius);
        rec.t = root;
        rec.Mat = Mat;
        
        return true;
        */
    }

    public override AABB WorldBound(float time0, float time1)
    {
        return new AABB(
            Center - new Vector3(Radius, Radius, Radius), 
            Center + new Vector3(Radius, Radius, Radius)
        );
    }

    public (float, float) GetSphereUV(Vector3 p)
    {
        float phi = (float)(Math.Atan2(p.Z, p.X) + Math.PI);
        float theta = (float)Math.Acos(p.Y);

        float u = phi / (2f*(float)Math.PI);
        float v = theta / (float)Math.PI;

        return (u, v);
    }
}