using UnityEngine;

[System.Serializable]
public class MouseLook{
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool smooth;
    public float smoothTime = 5f;
    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    public void Init(Transform character, Transform camera){
        m_CharacterTargetRot = character.rotation;
        m_CameraTargetRot = camera.localRotation;
    }
    public void LookRotation(Vector2 input, Transform character, Transform camera){
        float yRot = input.x * XSensitivity * Time.deltaTime;
        float xRot = input.y * YSensitivity * Time.deltaTime;

        m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);

        if(clampVerticalRotation) m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);
    }
    public void UpdateLookRotation(Transform character, Transform cam_trans){
        if(smooth){
            character.rotation = Quaternion.Slerp (character.rotation, m_CharacterTargetRot,
                smoothTime * Time.deltaTime);
            cam_trans.localRotation = Quaternion.Slerp (cam_trans.localRotation, m_CameraTargetRot,
                smoothTime * Time.deltaTime);
        }
        else{
            character.rotation = m_CharacterTargetRot;
            cam_trans.localRotation = m_CameraTargetRot;
        }
    }
    Quaternion ClampRotationAroundXAxis(Quaternion q){
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;
        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);
        angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);
        q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);
        return q;
    }
}