using UnityEngine;

public class WaterAnimation : MonoBehaviour


{
    public Material waterMaterial;
    public float speedX = 0.05f;
    public float speedY = 0.02f;

    public float slowFactor = 0.2f;

    void Update()
    {
        

        Vector2 offset = waterMaterial.mainTextureOffset;

        float finalSpeedX = speedX * slowFactor;
        float finalSpeedY = speedY * slowFactor;

        offset.x += finalSpeedX * Time.deltaTime;
        offset.y += finalSpeedY * Time.deltaTime;

        waterMaterial.mainTextureOffset = offset;
    }
}
