using System.Numerics;

public class BVH {
    public List<Hittable> Primitives { get; set; }
    public BVHNode Root { get; set; }

    public BVH(in List<Hittable> primitives, int start, int end)
    {
        if (  primitives.Count < 1 )
        {
            Console.Error.WriteLine("Tried to construct BVH from empty world!");
            Environment.Exit(1);
        }
        Primitives = primitives;

        // build primitives info
        List<BVHPrimitiveInfo> primitiveInfo = new List<BVHPrimitiveInfo>(primitives.Count);
        for ( int i = 0; i <= end; ++i )
        {
            primitiveInfo.Add(new BVHPrimitiveInfo(i, primitives[i].WorldBound()));
        }

        Root = recursiveBuild(ref primitiveInfo, 0, primitives.Count-1);
    }
    public BVHNode recursiveBuild(ref List<BVHPrimitiveInfo> primitiveInfo, int start, int end)
    {
        Console.Error.WriteLine("recursing section: " + start + ", " + end);
        BVHNode node = new BVHNode();

        // group all primitives of current section to a bound if we return a leaf node
        AABB bounds = new AABB();
        for ( int i = start; i <= end; ++i )
        {
            bounds = AABB.Union(bounds, primitiveInfo[i].Bounds);
        }

        // get # of primitives in current section of primitives
        int nPrimitives = end - start + 1;

        // only 1 primitive, return it as a leaf
        if ( nPrimitives == 1 )
        {
            Console.Error.WriteLine("1 primitive, leafing: " + start);
            // select our single prim as leaf
            List<Hittable> leafPrimitives = new List<Hittable>{ Primitives[primitiveInfo[start].PrimitiveNumber] };
            node.InitLeaf(nPrimitives, bounds, leafPrimitives);
            return node;
        }
        // otherwise, setup info for binning SAH
        else
        {
            // build AABB for all centroids of current section
            // used to get longest axis and also to project each centroid to a bin along this axis
            AABB centroidBounds = new AABB();
            for ( int i = start; i <= end; ++i )
            {
                centroidBounds = AABB.Union(centroidBounds, primitiveInfo[i].Centroid);
            }
            // select bins to span longest axis
            int dim = centroidBounds.MaximumExtent();
            
            int mid = (start + end + 1) / 2;
            // case where there's no volume, maybe an infinite plane? so return a leaf
            if ( centroidBounds.Maximum[dim] == centroidBounds.Minimum[dim] )
            {
                Console.Error.WriteLine("Infinte plane, leafing: " + start + " - " + end);
                // get all individual leaf prims from main list
                List<Hittable> leafPrimitives = new List<Hittable>(nPrimitives);
                for ( int i = start; i <= end; ++i )
                {
                    leafPrimitives.Add(Primitives[primitiveInfo[i].PrimitiveNumber]);
                }
                node.InitLeaf(nPrimitives, bounds, leafPrimitives);
                return node;
            }
            else
            {
                // not worth calculating buckets if only a few primitives in section
                if ( nPrimitives < 5 )
                {
                    Console.Error.WriteLine("nPrimitives < 5, leafing: " + start + "-" + end);
                    List<Hittable> leafPrimitives = new List<Hittable>(nPrimitives);
                    for ( int i = start; i <= end; ++i )
                    {
                        leafPrimitives.Add(Primitives[primitiveInfo[i].PrimitiveNumber]);
                    }
                    node.InitLeaf(nPrimitives, bounds, leafPrimitives);
                    return node;
                    /* instead, split primitives equally
                    Console.Error.WriteLine($"start: {start}, end: {end}, mid: {mid}");
                    Util.NthElement(ref primitiveInfo, start, end, mid, Comparer<BVHPrimitiveInfo>.Create((a, b) => {
                        return a.Centroid[dim].CompareTo(b.Centroid[dim]);
                    }));*/
                }
                else
                {
                    Console.Error.WriteLine("Bucketing...");
                    // setup number of buckets and their info like inner prim bounds, count of prims inside
                    int nBuckets = 12;
                    BucketInfo[] buckets = new BucketInfo[nBuckets];
                    for ( int i = 0; i < nBuckets; ++i )
                    {
                        // C# only inits capacity, have to init new objects
                        buckets[i] = new BucketInfo();
                    }

                    // project every primitive in curr section into each bucket along longest axis
                    for ( int i = start; i <= end; ++i ) 
                    {
                        int b = (int)(nBuckets * (1.0f/(centroidBounds.Maximum[dim]-centroidBounds.Minimum[dim])) 
                            * (primitiveInfo[i].Centroid[dim]-centroidBounds.Minimum[dim]));

                        if (b == nBuckets) b = nBuckets - 1;

                        ++buckets[b].Count;
                        buckets[b].Bounds = AABB.Union(buckets[b].Bounds, primitiveInfo[i].Bounds);
                    }

                    // calculate cost of every bucket partition choice
                    float[] cost = new float[nBuckets - 1];

                    for ( int i = 0; i < nBuckets - 1; ++i ) 
                    {
                        AABB b0 = new AABB();
                        AABB b1 = new AABB();
                        int count0 = 0, count1 = 0;
                        // from 0 to i, sum up these bucket's bounds and counts
                        for (int j = 0; j <= i; ++j) {
                            b0 = AABB.Union(b0, buckets[j].Bounds);
                            count0 += buckets[j].Count;
                        }
                        // from i+1 to end, sum up these bucket's bounds and counts
                        for (int j = i+1; j < nBuckets; ++j) {
                            b1 = AABB.Union(b1, buckets[j].Bounds);
                            count1 += buckets[j].Count;
                        }
                        // calculate the cost of partitioning at ith bucket
                        cost[i] = 0.125f + (count0 * b0.SurfaceArea() + count1 * b1.SurfaceArea()) / bounds.SurfaceArea();
                    }

                    // find the min cost bucket to split at
                    float minCost = cost[0];
                    int minCostSplitBucket = 0;
                    for ( int i = 1; i < nBuckets - 1; ++i ) 
                    {
                        if (cost[i] < minCost) {
                            minCost = cost[i];
                            minCostSplitBucket = i;
                        }
                    }
                    
                    Console.Error.WriteLine("minCost: " + minCost + ", splitBucket " + minCostSplitBucket);

                    // Either create leaf or split primitives at selected SAH bucket
                    // we set cost of intersection to be 1, so it == to number of prims
                    float leafCost = nPrimitives;
                    // we only group prims by their buckets if its cheaper than just intersecting all prims
                    if (minCost < leafCost) 
                    {
                        // partition primitives in current section so that order is [prims <= minCostSplitBucket < prims]
                        mid = Util.Partition(ref primitiveInfo, start, end, (pi) => {
                            // project centroid of all prims to a bucket along its longest dimension
                            int b = (int)(nBuckets * (1.0f/(centroidBounds.Maximum[dim]-centroidBounds.Minimum[dim])) 
                                * (pi.Centroid[dim]-centroidBounds.Minimum[dim]));
                            
                            // buckets are 0-indexed, so keep it between [0,nbuckets-1]
                            if (b == nBuckets) b = nBuckets - 1;

                            return b <= minCostSplitBucket;
                        }); 
                        
                        Console.Error.Write("mid: " + mid + ", [");
                        for ( int i = start; i <= end; ++i )
                        {
                            // project centroid of all prims to a bucket along its longest dimension
                            int b = (int)(nBuckets * (1.0f/(centroidBounds.Maximum[dim]-centroidBounds.Minimum[dim])) 
                                * (primitiveInfo[i].Centroid[dim]-centroidBounds.Minimum[dim]));
                            
                            // buckets are 0-indexed, so keep it between [0,nbuckets-1]
                            if (b == nBuckets) b = nBuckets - 1;
                            Console.Error.Write(b + ", ");
                        }
                        Console.Error.WriteLine("]\n");
                    } 
                    // cheaper to just create a leaf node
                    else 
                    {
                        Console.Error.WriteLine("After bucket leafing: " + start + " - " + end);
                        // get all individual leaf prims from main list
                        List<Hittable> leafPrimitives = new List<Hittable>(nPrimitives);
                        for ( int i = start; i <= end; ++i )
                        {
                            leafPrimitives.Add(Primitives[primitiveInfo[i].PrimitiveNumber]);
                        }
                        node.InitLeaf(nPrimitives, bounds, leafPrimitives);
                        return node;
                    }
                }
            }

            // recursively build subtrees of BVHNodes and eventually to leaves
            node.InitInterior(
                recursiveBuild(ref primitiveInfo, start, mid-1),
                recursiveBuild(ref primitiveInfo, mid, end)
            );
        }   

        return node;
    }
}

public class BucketInfo {
    public BucketInfo()
    {
        Count = 0;
        Bounds = new AABB();
    }
    public int Count { get; set; }
    public AABB Bounds { get; set; }
}

//BVHPrimitiveInfo structure
public class BVHPrimitiveInfo {
    public int PrimitiveNumber { get; set; }
    public AABB Bounds { get; set; }
    public Vector3 Centroid { get; set; }

    public BVHPrimitiveInfo(int primNumber, AABB bounds)
    {
        PrimitiveNumber = primNumber;
        Bounds = bounds;
        Centroid = 0.5f * (Bounds.Minimum + Bounds.Maximum);
    }
}