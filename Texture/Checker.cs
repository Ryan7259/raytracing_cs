using System.Numerics;

public class Checker : Texture
{
    public Texture Odd { get; set; }
    public Texture Even { get; set; }

    public Checker(Texture e, Texture o)
    {
        Even = e;
        Odd = o;
    }

    public Checker(Vector3 ec, Vector3 oc)
    {
        Even = new SolidColor(ec);
        Odd = new SolidColor(oc);
    }

    public override Vector3 Value(float u, float v, Vector3 p)
    {
        double sines = Math.Sin(10.0*p.X)*Math.Sin(10.0*p.Y)*Math.Sin(10.0*p.Z);
        if ( sines < 0 )
        {
            return Odd.Value(u, v, p);
        }
        else
        {
            return Even.Value(u, v, p);
        }
    }
}