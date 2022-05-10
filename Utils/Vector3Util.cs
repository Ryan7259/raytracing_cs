using System.Numerics;

public class Vector3Util {
    public static Vector3 Random() {
        return new Vector3(Util.Rand_Float(), Util.Rand_Float(), Util.Rand_Float());
    }
    public static Vector3 Random(float min, float max)
    {
        return new Vector3(Util.Rand_Float(min, max), Util.Rand_Float(min, max), Util.Rand_Float(min, max));
    }

    public static Vector3 Random_In_Unit_Sphere()
    {
        while ( true )
        {
            Vector3 p = Vector3Util.Random(-1.0f, 1.0f);
            if ( p.LengthSquared() < 1.0f )
            {
                return p;
            }
        }
    }
    public static Vector3 Random_In_Unit_Vector() // random point on unit sphere
    {
        return Vector3.Normalize(Random_In_Unit_Sphere());
    }
    public static Vector3 Random_In_Hemisphere(Vector3 normal) // random point in hemisphere of normal
    {
        Vector3 in_unit_sphere = Random_In_Unit_Sphere();
        if ( Vector3.Dot(in_unit_sphere, normal) > 0.0f )
        {
            return in_unit_sphere;
        }
        else
        {
            return -in_unit_sphere;
        }
    }
    public static Vector3 Random_In_Unit_Disk()
    {
        Vector3 in_unit_disk = Random_In_Unit_Sphere();
        in_unit_disk.Z = 0.0f;

        return in_unit_disk;
    }

    public static bool NearZero(in Vector3 a)
    {
        const float limit = 1e-8f;
        return Math.Abs(a.X) < limit && Math.Abs(a.Y) < limit && Math.Abs(a.Z) < limit;
    }

    // returns the reflected ray of v mirroring the normal n
    public static Vector3 Reflect(in Vector3 v, in Vector3 n)
    {
        return v - 2.0f * Vector3.Dot(v, n) * n;
    }

    // returns refracted ray across normal with index of refraction ir
    public static Vector3 Refract(in Vector3 i, in Vector3 n, in float ir)
    {
        float cos_theta = Vector3.Dot(-i, n);

        Vector3 t_perp = ir * (i + cos_theta*n);
        Vector3 t_parallel = (float)-Math.Sqrt(Math.Max(0.0f, 1.0f - t_perp.LengthSquared()))*n;

        return t_perp + t_parallel;
    }
}

