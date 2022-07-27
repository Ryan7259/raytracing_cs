using System.Numerics;

public class PerlinNoise : Texture
{
    public Perlin Noise {get; set;}
    public float Scale {get; set;}

    public PerlinNoise()
    {
        Noise = new Perlin();
        Scale = 1f;
    }

    public PerlinNoise(float scale)
    {
        Noise = new Perlin();
        Scale = scale;
    }

    public override Vector3 Value(float u, float v, Vector3 p)
    {
        // Dot prod. range: [-1,1], normalize to [0,1] w/ *0.5+1([-1,1]) for gamma sqrt
        // return new Vector3(1f) * 0.5f * (1f + Noise.Noise(Scale*p));
        //return new Vector3(1f) * Noise.Turb(Scale*p);

        // Px,Py,Pz can have a freq/period factor; turb has a power factor; turb depth=>turb size
        return new Vector3(1f) * 0.5f * (1f + MathF.Sin(Scale*p.Z + 6f*Noise.Turb(p)));
    }
}