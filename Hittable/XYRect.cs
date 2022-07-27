using System.Numerics;

public class XYRect : Hittable {
    public float X0 { get; }
    public float X1 { get; }
    public float Y0 { get; }
    public float Y1 { get; }
    public float K { get; }
    public Material Mat { get; }


    public XYRect(float x0, float x1, float y0, float y1, float k, Material mat)
    {
        X0 = x0;
        X1 = x1;
        Y0 = y0;
        Y1 = y1;
        K = k;
        Mat = mat;
    }

    /*
        P(t) = r.Origin + r.Dir*t

        solve for t with plane z = k:
        Pz = r.Origin + r.DirZ*t
        (Pz - r.Origin)/r.DirZ = t

        then check if ray is within rect
        check if Px and Py are between x0,x1 and y0,y1:
        Px = r.Origin + r.DirX*t
        Py = r.Origin + r.DirY*t
    */
    public override bool Hit(in Ray r, float t_min, float t_max, ref HitRecord rec)
    {
        float t = (K - r.Origin.Z) / r.Dir.Z;
        if ( t < t_min || t > t_max ) return false;

        float x = r.Origin.X + r.Dir.X*t;
        float y = r.Origin.Y + r.Dir.Y*t;

        if ( x < X0 || X1 < x || y < Y0 || Y1 < y ) return false;

        rec.U = (x-X0)/(X1-X0);
        rec.V = (y-Y0)/(Y1-Y0);
        rec.Mat = Mat;
        rec.P = r.at(t);
        rec.T = t;
        rec.SetFaceNormal(r, new Vector3(0f,0f,1f));
        return true;
    }

    public override AABB WorldBound(float time0, float time1)
    {
        return new AABB(new Vector3(X0, Y0, K-0.0001f), new Vector3(X1, Y1, K+0.0001f));
    }
}