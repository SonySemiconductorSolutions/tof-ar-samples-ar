float distanceSq(float2 pt1, float2 pt2)
{
    float2 v = pt2 - pt1;
    return dot(v,v);
}

float minimum_distance(float2 v, float2 w, float2 p) {
    float l2 = distanceSq(v, w);
    float t = max(0, min(1, dot(p - v, w - v) / l2));
    float2 projection = v + t * (w - v);
    return distance(p, projection);
}

float4 culcWireFrame(float2 uv, float4 wireframeColor, float wireframeColorIntencity){
    float lineWidthInPixels = 0.5;
    float lineAntiaAliasWidthInPixels = 1;

    float2 uVector = float2(ddx(uv.x),ddy(uv.x));
    float2 vVector = float2(ddx(uv.y),ddy(uv.y));

    float vLength = length(uVector);
    float uLength = length(vVector);
    float uvDiagonalLength = length(uVector+vVector);

    float maximumUDistance = lineWidthInPixels * vLength;
    float maximumVDistance = lineWidthInPixels * uLength;
    float maximumUVDiagonalDistance = lineWidthInPixels * uvDiagonalLength;

    float leftEdgeUDistance = uv.x;
    float rightEdgeUDistance = (1.0-leftEdgeUDistance);

    float bottomEdgeVDistance = uv.y;
    float topEdgeVDistance = 1.0 - bottomEdgeVDistance;

    float minimumUDistance = min(leftEdgeUDistance,rightEdgeUDistance);
    float minimumVDistance = min(bottomEdgeVDistance,topEdgeVDistance);
    float uvDiagonalDistance = minimum_distance(float2(0.0,1.0),float2(1.0,0.0),uv);

    float normalizedUDistance = minimumUDistance / maximumUDistance;
    float normalizedVDistance = minimumVDistance / maximumVDistance;
    float normalizedUVDiagonalDistance = uvDiagonalDistance / maximumUVDiagonalDistance;


    float closestNormalizedDistance = min(normalizedUDistance,normalizedVDistance);
    closestNormalizedDistance = min(closestNormalizedDistance,normalizedUVDiagonalDistance);


    float lineAlpha = 1.0 - smoothstep(1.0,1.0 + (lineAntiaAliasWidthInPixels/lineWidthInPixels),closestNormalizedDistance);

    lineAlpha *= 1;

    return float4(1, 1, 1, lineAlpha) * wireframeColor * wireframeColorIntencity;
}