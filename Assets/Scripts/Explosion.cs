using UnityEngine;

public class Explosion : MonoBehaviour
{
    public static Explosion instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 0.95f);
    }


}
