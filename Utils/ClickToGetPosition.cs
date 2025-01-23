using UnityEngine;

public class ClickToGetPosition : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���콺 ���� Ŭ��
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 position = hit.point;
                Debug.Log("Clicked Position: " + position);
            }
        }
    }
}
