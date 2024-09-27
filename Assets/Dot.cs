using System;
using System.Numerics;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public float dotTopSpeed;

    public float dotAcceleration;

    private float dotSpeed;

    public float destinationOffset;

    public float spawnOffset;

    private UnityEngine.Vector3 myOffset;

    [SerializeField]public GameObject uiTarget;

    // public static DotUI dotUI;

    public static BigInteger pointsInDots;

    private BigInteger value = 1;

    public static UnityEngine.Vector3 baseScale = new UnityEngine.Vector3(1f, 1f, 1f);

    private void Awake()
    {
    }

    private void OnEnable()
    {
        dotSpeed = -1f;
        dotAcceleration = 3f;
        value = 1;
        pointsInDots += value;
        // try
        // {
        //     myOffset = new UnityEngine.Vector3(
        //         (float)(UnityEngine.Random.Range(0,5) * (double)destinationOffset) + destinationOffset,
        //         (float)(UnityEngine.Random.Range(0,5) * 2.0 * (double)destinationOffset) - 4f * destinationOffset,
        //         0f
        //     );
        // }
        // catch (NullReferenceException)
        // {
        //     // Handle the case where RandomController.unseededRandom is null
        //     myOffset = UnityEngine.Vector3.zero;
        // }
    }

    public void SetValue(BigInteger newValue)
    {
        pointsInDots -= value;
        value = newValue;
        pointsInDots += value;
    }

    private void FixedUpdate()
    {
        if (uiTarget == null)
        {
            Debug.LogWarning("uiTarget is not set.");
            return;
        }

        transform.position = UnityEngine.Vector3.MoveTowards(
            transform.position,
            uiTarget.transform.position + myOffset,
            dotSpeed * Time.deltaTime
        );

        if (UnityEngine.Vector3.Distance(transform.position, uiTarget.transform.position + myOffset) > 0.03f)
        {
            dotSpeed += dotAcceleration * Time.deltaTime;
            dotAcceleration += dotAcceleration * Time.deltaTime;
        }
        else
        {
            Arrived();
        }
    }

    private void Arrived()
    {
        // if (dotUI != null)
        // {
        //     dotUI.Bump(value);
        // }

        // pointsInDots -= value;

        // if (AudioManager.instance != null)
        // {
        //     AudioManager.instance.PlayDot();
        // }

        // gameObject.transform.localScale = baseScale;
        // gameObject.SetActive(false);

        // if (ObjectPooler.instance != null)
        // {
        //     ObjectPooler.instance.pooledObjects.Enqueue(gameObject);
        // }
        // else
        // {
        //     // If ObjectPooler is not available, destroy the object
        //     Destroy(gameObject);
        // }
    }
}
