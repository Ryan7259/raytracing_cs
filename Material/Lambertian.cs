using System.Numerics;

public class Lambertian : Material 
{
    public Texture Albedo { get; set; }

    public Lambertian(Vector3 albedo)
    {
        Albedo = new SolidColor(albedo);
    }
    public Lambertian(Texture tex)
    {
        Albedo = tex;
    }

    public override bool Scatter(in Ray r_in, ref HitRecord rec, ref Vector3 attenuation, ref Ray scattered)
    {
        Vector3 scatter_dir = rec.Normal + Vector3Util.Random_In_Unit_Vector();
        if ( Vector3Util.NearZero(scatter_dir) )
        {
            scatter_dir = rec.Normal;
        }
        scattered = new Ray(rec.P, scatter_dir, r_in.Time);
        attenuation = Albedo.Value(rec.U, rec.V, rec.P);
        //Console.Error.WriteLine(attenuation);
        return true;
    }
}