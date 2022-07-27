using System.Numerics;

public class Metal : Material
{
    public Texture Albedo { get; }
    public float Fuzz { get; }

    public Metal(Vector3 albedo, float fuzz)
    {
        Albedo = new SolidColor(albedo);
        Fuzz = fuzz < 1.0f ? fuzz : 1.0f; // prevents fuzz from scaling out of unit circle
    }

    public override bool Scatter(in Ray r_in, ref HitRecord rec, ref Vector3 attenuation, ref Ray scattered)
    {
        Vector3 reflected = Vector3.Reflect(r_in.Dir, rec.Normal);

        // scale direction of perfect reflection in a random dir by a fuzz factor
        scattered = new Ray(rec.P, reflected + Fuzz*Vector3Util.Random_In_Unit_Sphere(), r_in.Time);

        attenuation = Albedo.Value(rec.U, rec.V, rec.P);
        
        return Vector3.Dot(scattered.Dir, rec.Normal) > 0.0; // prevents reflection into surface (only possible for refraction)
    }
}