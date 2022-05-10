using System.Numerics;

public class Lambertian : Material 
{
    public Vector3 Albedo { get; set; }

    public Lambertian(Vector3 albedo)
    {
        Albedo = albedo;
    }

    public override bool scatter(in Ray r_in, ref HitRecord rec, out Vector3 attenuation, out Ray scattered)
    {
        Vector3 scatter_dir = rec.normal + Vector3Util.Random_In_Unit_Vector();
        if ( Vector3Util.NearZero(scatter_dir) )
        {
            scatter_dir = rec.normal;
        }
        scattered = new Ray(rec.p, scatter_dir, r_in.Time);
        attenuation = Albedo;
        //Console.Error.WriteLine(attenuation);
        return true;
    }
}