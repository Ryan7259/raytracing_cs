using System.Numerics;

public class Assorted {
    public static List<Hittable> Assorted_Scene() {
        DiffuseLight light = new DiffuseLight(new Vector3(7f));

        Vector3 center1 = new Vector3(400, 400, 200);
        Vector3 center2 = center1 + new Vector3(30,0,0);
        Lambertian moving_sphere_material = new Lambertian(new Vector3(0.7f, 0.3f, 0.1f));


        List<Hittable> world = new List<Hittable>() {
            new XZRect(123f, 423f, 147f, 412f, 554f, light),
            new MovingSphere(center1, center2, 0f, 1f, 50f, moving_sphere_material)
        };

        Lambertian ground = new Lambertian(new SolidColor(0.48f, 0.84f, 0.53f));
        for ( int i = 0; i < 20; ++i)
        {
            for ( int j = 0; j < 20; ++j )
            {
                float w = 100f;
                float x0 = -1000f + i*w;
                float z0 = -1000f + j*w;
                float y0 = 0;
                float x1 = x0+w;
                float y1 = Util.Rand_Float(1f,101f);
                float z1 = z0+w;

                world.Add(new Box(new Vector3(x0,y0,z0), new Vector3(x1,y1,z1), ground));
            }
        }

        return world;
    }
}