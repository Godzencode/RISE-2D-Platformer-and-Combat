using UnityEngine;

public class Saw : MonoBehaviour
{

    [SerializeField] private Transform[] points;
    private int pointIndex;
    [SerializeField]private float time;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pointIndex = 0;
        MoveSaw();
    }

    void MoveSaw()
    {
        LeanTween.move(gameObject , points[pointIndex], time).setEaseOutExpo().setOnComplete(MoveSaw);
        pointIndex ++;
        if (pointIndex >= points.Length)
        {
            pointIndex = 0;
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
{
    Debug.Log("Saw touched: " + collision.gameObject.name + " | tag: " + collision.gameObject.tag);

    if (collision.gameObject.CompareTag("Player"))
    {
        Debug.Log("Tag OK");
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            Debug.Log("PlayerController found");
            PlayerController.instance.TakeDamage(1);
        }
    }
}
}
