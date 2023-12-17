using System;
using Barotrauma.Items.Components;

namespace Barotrauma;

[AttributeUsage(AttributeTargets.Property)]
class Editable : Attribute
{
    public MultiEditType EditType;

    public int MaxLength;
    public int DecimalCount = 1;

    public int MinValueInt = int.MinValue, MaxValueInt = int.MaxValue;
    public float MinValueFloat = float.MinValue, MaxValueFloat = float.MaxValue;
    public bool ForceShowPlusMinusButtons = false;
    public float ValueStep;

    /// <summary>
    /// Labels of the components of a vector property (defaults to x,y,z,w)
    /// </summary>
    public string[] VectorComponentLabels;

    /// <summary>
    /// If a translation can't be found for the property name, this tag is used instead
    /// </summary>
    public string FallBackTextTag;

    /// <summary>
    /// Currently implemented only for int and bool fields. TODO: implement the remaining types (SerializableEntityEditor)
    /// </summary>
    public bool ReadOnly;

    public enum MultiEditType
    {
        Replace,
        Modify,
        Relative
    }

    public Editable(int maxLength = 20, MultiEditType multiEditType = MultiEditType.Replace)
    {
        MaxLength = maxLength;
        EditType = multiEditType;
    }

    public Editable(int minValue, int maxValue, MultiEditType multiEditType = MultiEditType.Replace)
    {
        MinValueInt = minValue;
        MaxValueInt = maxValue;
        EditType = multiEditType;
    }

    public Editable(float minValue, float maxValue, int decimals = 1, MultiEditType multiEditType = MultiEditType.Replace)
    {
        MinValueFloat = minValue;
        MaxValueFloat = maxValue;
        DecimalCount = decimals;
        EditType = multiEditType;
    }
}

[AttributeUsage(AttributeTargets.Property)]
sealed class InGameEditable : Editable
{
}

