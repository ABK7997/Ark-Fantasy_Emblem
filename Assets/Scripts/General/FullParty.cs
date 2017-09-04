using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullParty : Singleton<FullParty> {

    public List<PartyMember> party;

    protected FullParty() { }

    public List<PartyMember> getParty()
    {
        return party; 
    }

    void Start()
    {

    }
}
