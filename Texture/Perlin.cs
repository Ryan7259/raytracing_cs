using System.Numerics;

public class Perlin {
    private const int PointCount = 256;
    private const int Mask = PointCount-1;
    private Vector3[] Gradients {get; set;}
    private int[] PermTable {get;}

    public Perlin()
    {
        // fill with random floats for gradient at corners
        // improved perlin noise suggests using these 16(ez lo 4 bits calcs) preset dirs to avoid clumping (permutation randomness is enough)
        Gradients = new Vector3[16] {
            new Vector3(1,1,0),new Vector3(-1,1,0),new Vector3(1,-1,0),new Vector3(-1,-1,0),
            new Vector3(1,0,1),new Vector3(-1,0,1),new Vector3(1,0,-1),new Vector3(-1,0,-1),
            new Vector3(0,1,1),new Vector3(0,-1,1),new Vector3(0,1,-1),new Vector3(0,-1,-1),
            new Vector3(1,1,0),new Vector3(-1,1,0),new Vector3(0,-1,1),new Vector3(0,-1,1),
        };
        
        /*
        // shuffled indices, used to select consistent grid points, always wraps
        PermTable = new int[PointCount*2];
        for ( int i = 0; i < PointCount; ++i )
        {
            PermTable[i] = i;
            PermTable[i+PointCount] = i;
        }
        for ( int i = PointCount*2-1; i > 0; --i )
        {
            int j = Util.Rand_Int(0, i);
            (PermTable[i], PermTable[j]) = (PermTable[j], PermTable[i]);
        }

        /*
        Console.Error.Write("\nPermTable: { ");
        int digits = 0;
        foreach ( int p in PermTable )
        {
            if ( digits % 25 == 0 ) Console.Error.WriteLine();
            Console.Error.Write(p + ", ");
            ++digits;
        }
        Console.Error.WriteLine(" }\n");
        */
        PermTable = new int[512] { 151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 
            103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 
            26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 
            87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 
            77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 
            46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 
            187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 
            198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 
            255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 
            170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 
            172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 
            104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 
            241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 
            157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 
            93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,
            151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 
            103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 
            26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 
            87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 
            77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 
            46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 
            187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 
            198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 
            255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 
            170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 
            172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 
            104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 
            241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 
            157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 
            93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
        };
    }

    public float Noise(Vector3 p)
    {
        int x0 = (int)Math.Floor(p.X) & Mask;
        int y0 = (int)Math.Floor(p.Y) & Mask;
        int z0 = (int)Math.Floor(p.Z) & Mask;

        // high # of turb layers leads to infinity diffs(inf - inf), just set to 0
        float tx = float.IsFinite(p.X) ? (float)(p.X-Math.Floor(p.X)) : 0;
        float ty = float.IsFinite(p.Y) ? (float)(p.Y-Math.Floor(p.Y)) : 0;
        float tz = float.IsFinite(p.Z) ? (float)(p.Z-Math.Floor(p.Z)) : 0;

        int x1 = (x0+1) & Mask;
        int y1 = (y0+1) & Mask;
        int z1 = (z0+1) & Mask;
        
        float u = Smoothstep(tx);
        float v = Smoothstep(ty);
        float w = Smoothstep(tz);

        // 000,100,010,110,001,101,011,111
        // gradient corners
        Vector3 c000 = Gradients[Hash(x0,y0,z0) & 15];
        Vector3 c100 = Gradients[Hash(x1,y0,z0) & 15];

        Vector3 c010 = Gradients[Hash(x0,y1,z0) & 15];
        Vector3 c110 = Gradients[Hash(x1,y1,z0) & 15];

        Vector3 c001 = Gradients[Hash(x0,y0,z1) & 15];
        Vector3 c101 = Gradients[Hash(x1,y0,z1) & 15];

        Vector3 c011 = Gradients[Hash(x0,y1,z1) & 15];
        Vector3 c111 = Gradients[Hash(x1,y1,z1) & 15];

        // point - corners
        /*
            P's lens are limited to unit cube so total len is 1 from all corners
            meaning tx,ty also limited to [0,1]

            in 2D visualize , visualize the 4 P's directions signs w/ bot left as origin
            from bot left: point to top rightish, +X,+Y
            from top left: point to bot rightish, +X,-Y
            from top right: point to bot leftish, -X,-Y
            from bot right: point to top leftish, -X,+Y
        */
        Vector3 p000 = new Vector3(tx,ty,tz);
        Vector3 p100 = new Vector3(tx-1,ty,tz);

        Vector3 p010 = new Vector3(tx,v-1,tz);
        Vector3 p110 = new Vector3(tx-1,ty-1,tz);

        Vector3 p001 = new Vector3(tx,ty,tz-1);
        Vector3 p101 = new Vector3(tx-1,ty,tz-1);

        Vector3 p011 = new Vector3(tx,ty-1,tz-1);
        Vector3 p111 = new Vector3(tx-1,ty-1,tz-1);

        /*
            Lerp(
                Lerp(
                    Lerp(a, b, u),
                    Lerp(c, d, u),
                    v)
                Lerp(
                    Lerp(e, f, u),
                    Lerp(g, h, u),
                    v),
            w)
        */
        float res = Lerp(w, 
            Lerp(v,
                Lerp(u, Vector3.Dot(p000,c000), Vector3.Dot(p100,c100)),
                Lerp(u, Vector3.Dot(p010,c010), Vector3.Dot(p110,c110))
            ),
            Lerp(v,
                Lerp(u, Vector3.Dot(p001,c001), Vector3.Dot(p101,c101)),
                Lerp(u, Vector3.Dot(p011,c011), Vector3.Dot(p111,c111))
            )
        );
        if ( float.IsNaN(res) )
        {
            Console.Error.WriteLine("(float)(p.X-Math.Floor(p.X)):" + p.X + " - " + Math.Floor(p.X));
            Console.Error.WriteLine("(float)(p.Y-Math.Floor(p.Y)):" + p.Y + " - " + Math.Floor(p.Y));
            Console.Error.WriteLine("(float)(p.Z-Math.Floor(p.Z)):" + p.Z + " - " + Math.Floor(p.Z));

            Console.Error.WriteLine("xyz0:" + x0 + " " + y0 + " " + z0);
            Console.Error.WriteLine("xyz1:" + x1 + " " + y1 + " " + z1);

            Console.Error.WriteLine("t:" + tx + " " + tx + " " + tz);

            Console.Error.WriteLine("\n");
        }
        return res;
    }

    public float Turb(Vector3 p, int depth=5)
    {
        float freq = 1f;
        float amp = 1f;

        float accum = 0f;
        for ( int i = 0; i < depth; ++i )
        {
            accum += amp * Noise(p * freq);;
            freq *= 2f;
            amp *= 0.5f;
        }
        return MathF.Abs(accum);
    }

    public int Hash(int x, int y, int z)
    {
        return PermTable[PermTable[PermTable[x]+y]+z];
    }

    /* 
    (1-t)a + (t)b
    a-at+bt
    a+t(b-a)
    */
    public float Lerp(float t, float a, float b)
    {
        return a + t*(b-a);
    }

    /*
    6t^5 - 15t^4 + 10t^3
    t^3(t(6t - 15) + 10)
    */
    public float Smoothstep(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }
}