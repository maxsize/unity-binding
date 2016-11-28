using UnityEngine;
using UnityEngine.Assertions;
using System.Reflection;

public class Binding : Object
{
    internal object fromValue;
    internal string fromPropertyName;
    object toValue;
    string toPropertyName;
    object oldValue;
    public Binding (object fromValue, string fromPropertyName, object toValue, string toPropertyName)
    {
        this.fromValue = fromValue;
        this.fromPropertyName = fromPropertyName;
        this.toValue = toValue;
        this.toPropertyName = toPropertyName;
        assert();
        init();
    }

    ///<summary>
    //apply changes
    ///</summary>
    public void apply()
    {
        if (isChanged)
        {
            //apply changes
            try
            {
                object newValue = getValue(fromValue, fromPropertyName);
                while (true)
                {
                    if (applyProperty(newValue)) break;
                    if (applyField(newValue)) break;
                    if (applyMethod(newValue)) break;

                    Debug.LogError("Invalid binding destination: " + toPropertyName);
                    break;
                }
            }
            catch(System.Exception e)
            {
                Debug.Log(e);
            }
        }
    }

    public bool isChanged
    {
        get
        {
            return getValue(fromValue, fromPropertyName) != oldValue;
        }
    }

    private bool applyProperty(object newValue)
    {
        PropertyInfo info = toValue.GetType().GetProperty(toPropertyName);
        if (info != null)
        {
            info.SetValue(toValue, newValue, null);
            return true;
        }
        return false;
    }

    private bool applyField(object newValue)
    {
        FieldInfo info = toValue.GetType().GetField(toPropertyName);
        if (info != null)
        {
            info.SetValue(toValue, newValue);
            return true;
        }
        return false;
    }

    private bool applyMethod(object newValue)
    {
        MethodInfo info = toValue.GetType().GetMethod(toPropertyName);
        if (info != null)
        {
            info.Invoke(toValue, new object[1]{newValue});
            return true;
        }
        return false;
    }

    private void assert()
    {
        Assert.IsNotNull(fromValue);
        Assert.IsNotNull(fromPropertyName);
        Assert.IsNotNull(toValue);
        Assert.IsNotNull(toPropertyName);
    }

    private void init()
    {
        oldValue = getValue(fromValue, fromPropertyName);
    }

    private object getValue(object target, string propertyName)
    {
        FieldInfo info = target.GetType().GetField(propertyName);
        object value = info.GetValue(target);
        return value;
    }
}