using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] ParticleSystem hitParticlePrefab;
    private TrailRenderer trail;
    private LayerMask hitMask;
    private float despawnTime;

    private void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        hitMask = LayerMask.GetMask("Environment", "Monster");
    }

    public void Init(Vector3 velocity)
    {
        trail.Clear();
        despawnTime = Time.time + 5f;
        _ = StartCoroutine(CoMove(velocity));
    }

    private IEnumerator CoMove(Vector3 velocity)
    {
        //Vector3 gravity = Physics.gravity;
        Vector3 gravity = new Vector3(0f, -9.81f, 0f);
        while (true)
        {
            velocity += gravity * Time.deltaTime;
            Debug.DrawLine(transform.position, transform.position + velocity * Time.deltaTime);
            if (Physics.Raycast(transform.position, velocity, out RaycastHit hitInfo, velocity.magnitude * Time.deltaTime, hitMask))
            {
                ParticleSystem hitParticle = LeanPool.Spawn(hitParticlePrefab);
                hitParticle.transform.parent = hitInfo.transform;
                hitParticle.transform.position = hitInfo.point;
                hitParticle.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
                LeanPool.Despawn(hitParticle, 10f);
                break;
            }
            
            transform.Translate(velocity * Time.deltaTime, Space.World);

            if(Time.time > despawnTime)
            {
                break;
            }

            yield return null;
        }
        LeanPool.Despawn(this);
    }
}