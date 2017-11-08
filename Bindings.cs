using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Bindings : MonoBehaviour {

	private static Bindings _instance;

	List<Binding> autoBindings;
	List<Binding> invalidBindings;
	List<Binding> allBindings;

	public static Bindings Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(Bindings)) as Bindings;
				if (_instance) _instance.init();
			}
			return _instance;
		}
	}

    private void init()
    {
		autoBindings = new List<Binding>();
		invalidBindings = new List<Binding>();
		allBindings = new List<Binding>();
    }

    public void AddBinding(Binding binding, bool auto = false)
	{
		if (allBindings.Find(b => b == binding) != null)
			return;
		List<Binding> list = auto ? autoBindings : invalidBindings;
		list.Add(binding);
		allBindings.Add(binding);
	}

	public void RemoveBinding(Binding binding)
	{
		removeFromList(allBindings, binding);
		removeFromList(autoBindings, binding);
		removeFromList(invalidBindings, binding);
	}

	public void Invalid(object source, string propertyName)
	{
		List<Binding> list = findBindings(source, propertyName);
		foreach (var item in list)
		{
			if (!invalidBindings.Contains(item))
				invalidBindings.Add(item);
		}
	}

	List<Binding> findBindings(object value, string propertyName)
	{
		List<Binding> list = new List<Binding>();
		foreach (var item in allBindings)
		{
			if (item.fromValue == value && item.fromPropertyName == propertyName)
				list.Add(item);
		}
		return list;
	}

	void removeFromList(List<Binding> list, Binding binding)
	{
		if (list.Contains(binding))
		{
			list.RemoveAt(list.IndexOf(binding));
		}
	}
	
	// Update is called once per frame
	void Update () {
		apply();
	}

    private void apply()
    {
		applyList(autoBindings);
		applyList(invalidBindings);
		invalidBindings.Clear();
    }

    private void applyList(List<Binding> list)
    {
		foreach (var item in list)
		{
			item.apply();
		}
    }
}
