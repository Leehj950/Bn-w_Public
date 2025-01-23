using UnityEngine;

public class ClickToGetPosition : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
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
