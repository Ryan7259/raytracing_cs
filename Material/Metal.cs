using System.Numerics;

public class Metal : Material
{
    public Vector3 Albedo { get; }
    public float Fuzz { get; }

    public Metal(Vector3 albedo, float fuzz)
    {
        Albedo = albedo;
        Fuzz = fuzz < 1.0f ? fuzz : 1.0f; // prevents fuzz from scaling out of unit circle
    }

    public override bool scatter(in Ray r_in, ref HitRecord rec, out Vector3 attenuation, out Ray scattered)
    {
        Vector3 reflected = Vector3.Reflect(r_in.Dir, rec.normal);
        scattered = new Ray(rec.p, reflected + Fuzz*Vector3Util.Random_In_Unit_Sphere(), r_in.Time);

        attenuation = Albedo;
        
        return Vector3.Dot(scattered.Dir, rec.normal) > 0.0; // prevents reflection into surface (only possible for refraction)
    }
}