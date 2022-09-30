using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderTransform : MonoBehaviour
{
    public float waterLevel = 0f;
    public float speed = .01f;
    public GameObject water;

    void Update()
    {
        float step = speed * Time.deltaTime;
        water.transform.localPosition = Vector3.MoveTowards(water.transform.localPosition, new Vector3(0, waterLevel, 0),step);
    }

    public void AdjustWaterLevel(float newWaterLevel)
    {
        waterLevel = newWaterLevel;
    }
}
