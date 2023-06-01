using UnityEngine;

[AddComponentMenu("Dynamic Bone/Dynamic Bone Collider Fade")]

public class DynamicBoneColliderFade : DynamicBoneColliderBase {
#if UNITY_5
	[Tooltip("The radius of the sphere or capsule.")]
#endif	
    public float m_Radius = 0.5f;
    public float m_FadeRadius = 1f;
    [Range(0,1)]
    public float ForceMultiplier = 0f;
	
#if UNITY_5
	[Tooltip("The height of the capsule.")]
#endif		

    void OnValidate()
    {
        m_Radius = Mathf.Max(m_Radius, 0);
        m_FadeRadius = Mathf.Max(m_FadeRadius, m_Radius);
    }

    public override void Collide(ref Vector3 particlePosition, float particleRadius)
    {
        float radius = m_Radius * Mathf.Abs(transform.lossyScale.x);
        float radius_fade = m_FadeRadius * Mathf.Abs(transform.lossyScale.x);
        OutsideSphere(ref particlePosition, particleRadius, transform.TransformPoint(m_Center), radius, radius_fade, ForceMultiplier);
    }

    static void OutsideSphere(ref Vector3 particlePosition, float particleRadius, Vector3 sphereCenter, float sphereRadius, float fadeSphererRadius, float multiplier)
    {
        float m_r = fadeSphererRadius + particleRadius;
        float m_r2 = m_r * m_r;
        Vector3 d = particlePosition - sphereCenter;
        float fade = 1-(d.magnitude - sphereRadius)/(fadeSphererRadius - sphereRadius);
        fade = Mathf.Clamp01(fade);
        float len2 = d.sqrMagnitude;

        // if is inside sphere, project onto sphere surface
        if (len2 > 0 && len2 < m_r2)
        {
            float len = Mathf.Sqrt(len2);
            particlePosition = particlePosition + multiplier*.01f*fade * fade * d.normalized * (m_r / len);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!enabled)
            return;

        float radius = m_Radius * Mathf.Abs(transform.lossyScale.x);
        float fadeRadius = m_FadeRadius * Mathf.Abs(transform.lossyScale.x);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.TransformPoint(m_Center), radius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.TransformPoint(m_Center), fadeRadius);
    }
}
