using Photon.Pun;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed = 1f;
    public float minRotationAngle = -90f;
    public float maxRotationAngle = 90f;

    private float currentRotation = 0f;

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {

        if (!photonView.IsMine)
        {
            return;
        }

        float mouseY = Input.GetAxis("Mouse Y");
        float rotationAmount = -mouseY * rotationSpeed;

        // Cập nhật góc quay của camera
        currentRotation += rotationAmount;

        // Giới hạn góc quay của camera trong khoảng minRotationAngle và maxRotationAngle
        currentRotation = Mathf.Clamp(currentRotation, minRotationAngle, maxRotationAngle);

        // Áp dụng góc quay vào transform của camera
        transform.localRotation = Quaternion.Euler(currentRotation, 0f, 0f);
    }
}
