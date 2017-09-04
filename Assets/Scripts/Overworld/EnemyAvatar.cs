using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAvatar : Avatar {

    public List<Entity> party;

    public List<Entity> getParty()
    {
        return party;
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
