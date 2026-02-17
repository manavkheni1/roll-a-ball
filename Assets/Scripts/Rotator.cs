using UnityEngine;

public class Rotator : MonoBehaviour
{
    void Update()
    {
        // Capitalize 'Rotate'
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }
}