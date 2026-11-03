using EngineGDI;
using System;
using System.Collections.Generic;

public class CLSBulletPool
{
    public List<HERBullet> availableBullets = new List<HERBullet>();
    public List<HERBullet> activeBullets = new List<HERBullet>();

    public List<HERBullet> ActiveBullets => activeBullets;

    public CLSBulletPool(int MaxDesiredSize = 100)
    {
        for (int i = 0; i < MaxDesiredSize; i++)
        {
            HERBullet newBullet = new HERBullet();
            availableBullets.Add(new HERBullet());
            

        }
    }

    public HERBullet GetBullet(STRVector2f position, float angle, STRVector2f shipVelocity)
    {
        HERBullet bulletToUse; 

        if (availableBullets.Count > 0)
        {
            bulletToUse = availableBullets[0];
            availableBullets.RemoveAt(0);
        }
        else 
        {
            bulletToUse = new HERBullet(); 
        }

        bulletToUse.OnDeactivate -= OnBulletDeactivated;
        bulletToUse.OnDeactivate += OnBulletDeactivated;

        bulletToUse.Init(position, angle, shipVelocity);

        activeBullets.Add(bulletToUse);
        return bulletToUse;
    }

    private void OnBulletDeactivated(HERBullet bullet)
    {
        if (activeBullets.Contains(bullet))
        {
            activeBullets.Remove(bullet);
        }
        if (!availableBullets.Contains(bullet))
        {
            availableBullets.Add(bullet);
        }
    }

    public void Update(float deltaTime)
    {
        for (int i = activeBullets.Count - 1; i >= 0; i--)
        {
            activeBullets[i].Update(deltaTime);
        }
    }

    public void Draw()
    {
        foreach (var bullet in activeBullets)
        {
                bullet.Draw();
        }
    }
}
