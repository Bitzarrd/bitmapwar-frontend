﻿using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ExcelAssetAttribute : Attribute
{
    public string AssetPath { get; set; }
    public string ExcelName { get; set; }
    public bool LogOnImport { get; set; }
}