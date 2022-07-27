using System.Numerics;

class CornellBox {
    public static List<Hittable> Cornell_Box_Scene()
    {
        Lambertian red = new Lambertian(new Vector3(0.65f, 0.05f, 0.05f));
        Lambertian white = new Lambertian(new Vector3(0.73f));
        Lambertian green = new Lambertian(new Vector3(0.12f, 0.45f, 0.15f));
        DiffuseLight light = new DiffuseLight(new Vector3(15f));

        Hittable box1 = new Box(new Vector3(), new Vector3(165f, 330f, 165f), white);
        box1 = new RotateY(box1, 15f);
        box1 = new Translate(box1, new Vector3(265f, 0, 295f));

        Hittable box2 = new Box(new Vector3(), new Vector3(165f), white);
        box2 = new RotateY(box2, -18f);
        box2 = new Translate(box2, new Vector3(130f, 0, 65f));

        return new List<Hittable>() {
            new YZRect(0, 555f, 0, 555f, 555f, green),
            new YZRect(0, 555f, 0, 555f, 0, red),
            new XZRect(213f, 343f, 227f, 332f, 554f, light),
            new XZRect(0, 555f, 0, 555f, 0, white),
            new XZRect(0, 555f, 0, 555f, 555f, white),
            new XYRect(0, 555f, 0, 555f, 555f, white),
            box1,
            box2
        };
    }
}