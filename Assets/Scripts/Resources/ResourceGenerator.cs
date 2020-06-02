﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResourceTypes;

public class ResourceGenerator : MonoBehaviour
{
    public ResourceType type;
    public int income;
	public ResourceStorage destination;

    void FixedUpdate()
    {
		destination.AddResources(type, income);
    }
}
