using System.Numerics;

public class YZRect : Hittable {
    public float Y0 { get; }
    public float Y1 { get; }
    public float Z0 { get; }
    public float Z1 { get; }
    public float K { get; }
    public Material Mat { get; }


    public YZRect(float y0, float y1, float z0, float z1, float k, Material mat)
    {
        Y0 = y0;
        Y1 = y1;
        Z0 = z0;
        Z1 = z1;
        K = k;
        Mat = mat;
    }

    /*
        P(t) = r.Origin + r.Dir*t

        solve for t with plane x = k:
        Px = r.Origin + r.DirX*t
        (Px - r.Origin)/r.DirX = t

        then check if ray is within rect
        check if Px and Py are between y0,y1 and z0,z1:
        Py = r.Origin + r.DirY*t
        Pz = r.Origin + r.DirZ*t
    */
    public override bool Hit(in Ray r, float t_min, float t_max, ref HitRecord rec)
    {
        float t = (K - r.Origin.X) / r.Dir.X;
        if ( t < t_min || t > t_max ) return false;

        float y = r.Origin.Y + r.Dir.Y*t;
        float z = r.Origin.Z + r.Dir.Z*t;

        if ( z < Z0 || Z1 < z || y < Y0 || Y1 < y ) return false;

        rec.U = (y-Y0)/(Y1-Y0);
        rec.V = (z-Z0)/(Z1-Z0);
        rec.Mat = Mat;
        rec.P = r.at(t);
        rec.T = t;
        rec.SetFaceNormal(r, new Vector3(1f,0f,0f));
        return true;
    }

    public override AABB WorldBound(float time0, float time1)
    {
        return new AABB(new Vector3(K-0.0001f, Y0, Z0), new Vector3(K+0.0001f, Y1, Z1));
    }
}