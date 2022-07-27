/*
    beer-lambert's law describes how much of light/color is affected by
    distance and absorption coefficient, it follows a exponential graph
    where more distance and higher absorption decreases light contribution
        formula: exp(-distance*absorption)

    uniform ray marching for constant density volume:
        forward ray-march (better b/c we can stop when in-scatter contribution is near zero):
            Att *= Att for every iter => exponential decrease of in-scatter contribution
            Result = Li(x0)*Att 
                = + Li(x1)*Att^2 + Li(x0)*Att 
                = ... + Li(x2)*Att^3 + Li(x1)*Att^2 + Li(x0)*Att 
        backward ray-march:
            Result *= Att after adding in-scatter contribution
            Result = ( Li(x0)*Att )
                = ( Li(x0)*Att + Li(x1) )*Att
                = ( Li(x0)*Att^2 + Li(x1)*Att + Li(x2) )*Att + ...

*/