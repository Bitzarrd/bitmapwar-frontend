using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(AssetPath = "Resources")]
public class TestItems : ScriptableObject
{
	public List<TestItemEntity> Entities; // Replace 'EntityType' to an actual type that is serializable.
}
