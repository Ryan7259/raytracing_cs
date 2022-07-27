using System.Numerics;

class Dielectric : Material 
{

    // index of refraction is index of 1st(incident) medium / index of 2nd medium
    public float Ir { get; set; }

    public Dielectric(float ir)
    {
        Ir = ir;
    }

    public override bool Scatter(in Ray r_in, ref HitRecord rec, ref Vector3 attenuation, ref Ray scattered )
    {
        // get components to build refracted ray => T = A + B or T_perp + T_parallel or sin(phi)*Mhat + cos(phi)*(-Nhat)
        // Note: Mhat is perp dir vector to Nhat
        // Snell's Law: sin(theta)*etai = sin(phi)*etat
        // eta is refraction index ratio = etai/etat; i = incident ray medium; t = transmitted ray medium

        // since we only have material refraction index, we need to flip it for vacuum/air medium to object medium
        // this happens if hit record tells us ray hit a front face, or ray dir dot with normal was negative
        // if front_face is false, the ray is refracting out of a surface, so order is correct  as object medium TO vacuum/air medium
        float ir_ratio = rec.FrontFace ? 1.0f/Ir : Ir;

        // total internal reflection occurs when etat > eta1 and eta*sin(phi) = sin(theta) is unsolvable
        // arcsin(eta*sin(phi) > 1.0) is invalid since sin is between -1.0 and 1.0, 
        // this tells us there is no refracted ray, only reflection
        float cos_theta = Math.Min(Vector3.Dot(-r_in.Dir, rec.Normal), 1.0f);
        float sin_theta = (float)Math.Sqrt(1.0f - cos_theta*cos_theta);

        // or we could also just check if sqrt of parallel component is solvable/possible
        //float inner = 1.0 - Ir*Ir*(1.0 - cos_theta*cos_theta)
        // and check if ( inner < 0.0 )

        // reflect everything
        bool cannot_refract = ir_ratio * sin_theta > 1.0;
        
        /*
            Schlick's approximation to get reflection ray quicker than fresnel equations
            R(theta) = R0 + (1-R0)(1-cos(theta))^5
            R0 = (etai - etat / etai + etat)^2
        */

        Vector3 scatter_dir;
        if ( cannot_refract || Shlick(cos_theta, ir_ratio) > Util.Rand_Float() )
        {
            scatter_dir = Vector3.Reflect(r_in.Dir, rec.Normal);
        }
        else
        {
            scatter_dir = Vector3Util.Refract(r_in.Dir, rec.Normal, ir_ratio);
        }

        scattered = new Ray(rec.P, scatter_dir, r_in.Time);
        attenuation = new Vector3(1.0f);

        return true;
    }

    private static float Shlick(float cos_theta, float ir_ratio)
    {
        float r0 = (1.0f - ir_ratio) / (1 + ir_ratio); // ratio order doesn't seem to affect final result
        r0 = r0*r0;

        return r0 + (1.0f - r0) * (float)Math.Pow(1.0f - cos_theta, 5.0f);
    }
}