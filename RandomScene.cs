using System.Numerics;

class RandomScene {
    public static List<Hittable> Random_Scene()
    {
        List<Hittable> world = new List<Hittable>();

        Material ground_material = new Lambertian(new Vector3(0.5f, 0.5f, 0.5f));
        world.Add(new Sphere(new Vector3(0.0f, -1000.0f, 0.0f), 1000.0f, ground_material));
        for ( int a = -11; a < 11; ++a )
        {
            for ( int b = -11; b < 11; ++b )
            {
                float rand_mat = Util.Rand_Float();
                Vector3 center = new Vector3(a + 0.9f*Util.Rand_Float(), 0.2f, b + 0.9f*Util.Rand_Float());

                if ((center - new Vector3(4f, 0.2f, 0f)).Length() > 0.9) 
                {
                    if ( rand_mat < 0.8 )
                    {
                        Vector3 albedo = Vector3Util.Random();
                        albedo *=  Vector3Util.Random();
                        Material sph_mat = new Lambertian(albedo);
                        world.Add(new Sphere(center, 0.2f, sph_mat));
                    }
                    else if ( rand_mat < 0.95 )
                    {
                        Vector3 albedo = Vector3Util.Random(0.5f, 1.0f);
                        float fuzz = Util.Rand_Float(0.0f, 0.5f);
                        Material sph_mat = new Metal(albedo, fuzz);
                        world.Add(new Sphere(center, 0.2f, sph_mat));
                    }
                    else
                    {
                        Material sph_mat = new Dielectric(1.5f);
                        world.Add(new Sphere(center, 0.2f, sph_mat));
                    }
                }
            }
        }
        Material mat1 = new Dielectric(1.5f);
        world.Add(new Sphere(new Vector3(0.0f, 1.0f, 0.0f), 1.0f, mat1));

        Material mat2 = new Lambertian(new Vector3(0.4f, 0.2f, 0.1f));
        world.Add(new Sphere(new Vector3(-4.0f, 1.0f, 0.0f), 1.0f, mat2));

        Material mat3 = new Metal(new Vector3(0.7f, 0.6f, 0.5f), 0.0f);
        world.Add(new Sphere(new Vector3(4.0f, 1.0f, 0.0f), 1.0f, mat3));

        return world;
    }
}