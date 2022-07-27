using System.Numerics;

public class XZRect : Hittable {
    public float X0 { get; }
    public float X1 { get; }
    public float Z0 { get; }
    public float Z1 { get; }
    public float K { get; }
    public Material Mat { get; }


    public XZRect(float x0, float x1, float z0, float z1, float k, Material mat)
    {
        X0 = x0;
        X1 = x1;
        Z0 = z0;
        Z1 = z1;
        K = k;
        Mat = mat;
    }

    /*
        P(t) = r.Origin + r.Dir*t

        solve for t with plane y = k:
        Py = r.Origin + r.DirY*t
        (Py - r.Origin)/r.DirY = t

        then check if ray is within rect
        check if Px and Py are between x0,x1 and y0,y1:
        Px = r.Origin + r.DirX*t
        Pz = r.Origin + r.DirZ*t
    */
    public override bool Hit(in Ray r, float t_min, float t_max, ref HitRecord rec)
    {
        float t = (K - r.Origin.Y) / r.Dir.Y;
        if ( t < t_min || t > t_max ) return false;

        float x = r.Origin.X + r.Dir.X*t;
        float z = r.Origin.Z + r.Dir.Z*t;

        if ( x < X0 || X1 < x || z < Z0 || Z1 < z ) return false;

        rec.U = (x-X0)/(X1-X0);
        rec.V = (z-Z0)/(Z1-Z0);
        rec.Mat = Mat;
        rec.P = r.at(t);
        rec.T = t;
        rec.SetFaceNormal(r, new Vector3(0f,1f,0f));
        return true;
    }

    public override AABB WorldBound(float time0, float time1)
    {
        return new AABB(new Vector3(X0, K-0.0001f, Z0), new Vector3(X1, K+0.0001f, Z1));
    }
}