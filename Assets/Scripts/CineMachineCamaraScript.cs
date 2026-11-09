using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Rendering;

public class CineMachineCamaraScript : MonoBehaviour
{
    public static CineMachineCamaraScript instance;
    private CinemachineCamera cinemachineCamera;
    private float shakeStartTime;
    [SerializeField] private bool timer;
    private CinemachineBasicMultiChannelPerlin perlin;

    private void Awake()
    {
        if (instance!= null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        cinemachineCamera = gameObject.GetComponent<CinemachineCamera>();
        perlin = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeStartTime <= 0 )
        {
            perlin.AmplitudeGain = 0;
        }
        else
        {
            shakeStartTime -= Time.deltaTime;
        }

    }

    public void CameraShake(float intencity, float duration)
    {
        
        shakeStartTime = duration;
        perlin.AmplitudeGain = intencity;

        
    }
}

