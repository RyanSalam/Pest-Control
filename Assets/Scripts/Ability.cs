using System.Collections;
using System.Collections.Generic;

public abstract class Ability
{

}

public abstract class test 
{
    projectile whatProjIwannaSPawn;

    public abstract void shoot();

    public void raycast()
    {

    }

    public void projectile()
    {
        
    }

}

class grenadelauncher : test
{
    public override void shoot()
    {
        projectile();
    }
}

class projectile
{
    void update()
    {

    }
}