using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : OBJPool<Projectile>
{
    public static ProjectilePool Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }
    protected override Projectile MakeNewInstance()
    {
        Projectile newProjectile = Instantiate(mOrigin);
        mPool.Add(newProjectile);
        newProjectile.gameObject.SetActive(false); 

        return newProjectile;
    }
}
