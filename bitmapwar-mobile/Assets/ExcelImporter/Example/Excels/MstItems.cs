using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class MstItems : ScriptableObject
{
	public List<MstItemEntity> Entities; // Replace 'EntityType' to an actual type that is serializable.
}
