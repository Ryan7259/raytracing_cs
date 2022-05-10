using System.Numerics;

public class Camera {

    public Vector3 Origin { get; set; }
    public Vector3 LowerLeftCorner { get; set; }
    public Vector3 Horizontal { get; set; }
    public Vector3 Vertical { get; set; }

    public float AspectRatio { get; set; }
    public float ViewportHeight { get; set; }
    public float ViewportWidth { get; set; }
    public float LensRadius { get; set; }

    // unit vectors of camera direction axes
    public Vector3 U { get; set; }
    public Vector3 V { get; set; }
    public Vector3 W { get; set; }

    public float Time0 { get; set; }
    public float Time1 { get; set; }

    public Camera(
        Vector3 lookfrom, 
        Vector3 lookat, 
        Vector3 vup, 
        float vfov, 
        float aspect_ratio,
        float aperture,
        float focus_dist,
        float time0,
        float time1
    ) 
    {
        AspectRatio = aspect_ratio;
        /*  ^ +y
            |  /|       
            | / | h/2 = 1
            |/  |
       +z<--------> -z   
            |\ vfov/2
            | \
            |  \
            zoom = 1/tan(fov/2)
            fov = 2 arctan(1/zoom)
            zoom and fov are inversely proportional showing their behavior of wide fov meaning we see more (small zoom) and vice versa

            tan(vfov/2) = h/2d; 
            d = h/2tan(vfov/2); 
            h = 2*d*tan(vfov/2)

            default ratio is fov of 90 deg for w/h of [-1,1] => length = 2 and focal length = 1 for simplicity

            for Depth of Field, we need to know that focus distance and aperture are different from near plane and focal length
            focal length - distance from camera/eyes to projection plane
            focus dist - distance from lens where objects are in focus (or dist of proj plane from camera origin)
            aperture - radius determines the broadness of focused objects, large means only small part of scene, small means a large part of scene is focused
            proj. plane - sort of like a window/grid of pixels where a ray traced shot from camera origin through each pane/square towards any objects to influence its color

            in 3d world, we set eye/camera origin at aperture/lens position; also use z near plane w/ a focal length of 1 for simplicty
            aperture sizes are different from image proj plane sizes, but we can use similar triangles to solve eachother

            for DoF, we want to push our 'lens' or proj plane a focus dist away towards -z axis so its right in front of an object at that point
            all rays from a disk radius will hit object at that focus distance at same pt. so image is clearer
            for other distant objs, their color info when hit will be spread out among diff rays that happen to intersect them on their way to focus dist pt
                
            below, | is the focus dist/proj pane; so objects at this plane will all have rays hitting same point while objects not at that plane will be defocused
                camera shoots from random disk O>|< rays go outwards hitting multiple objs to influence one pixel meaning it is less focused
        */
        
        vfov = Util.Deg_To_Rad(vfov);
        float h = (float)(2.0*focus_dist*Math.Tan(vfov/2.0)); // we * by 2 b/c we are doing tan on one half of fov cone

        ViewportHeight = h;
        ViewportWidth = AspectRatio * ViewportHeight;
        
        // A X B, A => lookfrom - lookat, b/c this points our idx finger in a direction where our thumb (cross prod) is facing us
        // rhr (idx = +y, thumb = +x, mid = +z)
        W = Vector3.Normalize(lookfrom-lookat);
        U = Vector3.Normalize(Vector3.Cross(vup, W));
        V = Vector3.Cross(W, U);

        Origin = lookfrom;
        Horizontal = U * ViewportWidth;
        Vertical = V * ViewportHeight;

        /*
            lookfrom and lookat are in world space and have nothing to do with camera projection vecs
            camera builds a projection axes of its own based on lookfrom/lookat
            projects it onto a near plane that's a focal length dist from origin of camera => cam_orig + (-w*focal length)
            and near plane is also viewport width/height which is derived from aspect ratio, fov, and focal dist
            far plane is infinite for this program ( used for depth testing when tracing rays thru world objs )
        */
        LowerLeftCorner = Origin - W*focus_dist - Horizontal/2.0f - Vertical/2.0f;

        LensRadius = aperture/2.0f;
        Time0 = time0;
        Time1 = time1;
    }

    /* 
        used to generate a ray with depth of field
        s and t represent the dist ratio from min(lower left corner) to max horiz/vert values
    */
    public Ray Get_Ray(float s, float t)
    {
        // generate random point on unit disk scaled to a radius amount
        Vector3 in_disk = LensRadius * Vector3Util.Random_In_Unit_Disk();

        // scale along unit vecs of camera space axes setup first
        // since just adding will only offset along world space axes setup
        Vector3 offset = U*in_disk.X + V*in_disk.Y;

        return new Ray(
            // offset origin in camera space
            Origin + offset, 
            // (target pt) - origin => ray direction
            (LowerLeftCorner + s*Horizontal + t*Vertical) - Origin - offset, 
            Util.Rand_Float(Time0, Time1)
        );
    }
}