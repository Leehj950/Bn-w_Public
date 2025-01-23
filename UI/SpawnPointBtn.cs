using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPointBtn : MonoBehaviour
{
    public ParticleSystem leftLaneParticle;
    public ParticleSystem rightLaneParticle;
    public Button spawnPointBtn;


    public bool isLeft = true;

    public void Start()
    {
        isLeft = true;
        HighlightLane(isLeft);
    }

    public void ChangeDirection()
    {
        isLeft = !isLeft;
        HighlightLane(isLeft);
    }

    public void HighlightLane(bool isLeft)
    {
        if (isLeft)
        {
            leftLaneParticle.Play();
            rightLaneParticle.Stop();
        }
        else
        {
            rightLaneParticle.Play();
            leftLaneParticle.Stop();
        }
    }
}