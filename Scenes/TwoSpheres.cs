using System.Numerics;

class TwoSpheres {
    public static List<Hittable> Two_Spheres() {
        Texture checker = new Checker(new Vector3(.2f,.3f,.1f), new Vector3(.9f, .9f, .9f));
        
        return new List<Hittable>() {
            new Sphere(new Vector3(0f, -10f, 0f), 10f, new Lambertian(checker)),
            new Sphere(new Vector3(0f, 10f, 0f), 10f, new Lambertian(checker))
        };
    }
}
