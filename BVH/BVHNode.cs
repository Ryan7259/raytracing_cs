/*
    hittable list holds primitives themselves
    from this we can create a binary tree to hold BVH Nodes that are 
    split/parititoned by randomness/extent of axes or other heuristics like Binned SAH

    we partition our primitives into AABBs so we don't have to test every ray against every primitive
    this cuts down our ray tests to O(log(n)) instead of O(n)

    ray trace test hit flow:
        bvhnode(AABB of entire hittable list)
             /       \
      bvhnode(AABB)   bvhnode(AABB)
          /   \         \
      sphere  sphere    sphere
*/  
public class BVHNode : Hittable {
    public BVHNode? Left { get; set; }
    public BVHNode? Right { get; set; }
    public List<Hittable> Primitives { get; set; }
    public AABB Bounds { get; set; }
    public int NPrimitives { get; set; }
    
    public BVHNode()
    {
        Primitives = new List<Hittable>();
        NPrimitives = 0;
        Bounds = new AABB();
    }
    public void InitLeaf(int n, AABB b, List<Hittable> primitives)
    {
        Primitives = primitives;
        NPrimitives = n;
        Bounds = b;
    }
    public void InitInterior(BVHNode c0, BVHNode c1)
    {
        Left = c0;
        Right = c1;
        Bounds = AABB.Union(c0.Bounds, c1.Bounds);
        NPrimitives = 0;
    }

    public override bool hit(in Ray r, float t_min, float t_max, ref HitRecord rec)
    {
        // do AABB test, early out if doesn't intersect
        if ( !Bounds.hit(r, t_min, t_max) ) return false;
        
        // check if it's a leaf
        if ( Left == null && Right == null )
        {
            // intersect test against all the primitives inside, hit record only leaves closest hit info
            HitRecord temp_rec = new HitRecord();
            bool hit_anything = false;
            float closest_t_so_far = t_max;

            foreach ( Hittable h in Primitives )
            {
                if ( h.hit(r, t_min, closest_t_so_far, ref temp_rec) )
                {
                    // atleast hit 1 object
                    hit_anything = true;
                    closest_t_so_far = temp_rec.t;
                    rec = temp_rec;
                }
            }
            return hit_anything;
        }
        // not a leaf, test ray againt children
        else
        {
            bool hit_left = Left != null ? Left.hit(r, t_min, t_max, ref rec) : false;

            // since hit also updates farthest object to a closer val, 
            // we should set it to a potentially updated tmax from recursing left children
            bool hit_right = Right != null ? Right.hit(r, t_min, hit_left ? rec.t : t_max, ref rec) : false;

            return hit_left || hit_right;
        }
    }

    public override AABB WorldBound(float time, float time1)
    {
        return Bounds;
    }
}