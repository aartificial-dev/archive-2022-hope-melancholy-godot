using System;

[System.AttributeUsage(
    System.AttributeTargets.Struct |
    System.AttributeTargets.Field
    )]
public class Saveable: System.Attribute {

}