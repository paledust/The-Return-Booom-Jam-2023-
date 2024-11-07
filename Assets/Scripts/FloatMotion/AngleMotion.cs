using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleMotion : MonoBehaviour
{
    [SerializeField] private float maxAngle;
    [SerializeField] private float freq;
    [SerializeField] private Vector3 axis = Vector3.forward;

    private float timer = 0;
    private float seed;
    private Quaternion initRot;

    // Start is called before the first frame update
    void Start()
    {
        seed = Random.Range(-1f, 1f);
        initRot = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime*freq;

        transform.localRotation = Quaternion.Euler(axis * Mathf.Sin((timer + seed)*Mathf.PI) * maxAngle)*initRot;
    }
}
