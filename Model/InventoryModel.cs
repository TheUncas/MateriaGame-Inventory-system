﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#region Delegates
public delegate void InventoryModelEventHandler<T>(InventoryModel<T> pInventory, InventorySlotModel<T> pInventorySlot) where T : IInventoriable;
#endregion

/// <summary>
/// Representation of the inventory
/// </summary>
[Serializable]
public class InventoryModel<T> where T : IInventoriable
{

    #region events
    public InventoryModelEventHandler<T> onObjectAdded, onObjectRemoved, onObjectGot, onSlotCreated, onObjectReplace;
    #endregion

    #region Properties

    /// <summary>
    /// Number of allowed slot
    /// </summary>
    [SerializeField]
    public int numberOfSlot;

    /// <summary>
    /// List of inventory slot
    /// </summary>
    [SerializeField]
    public List<InventorySlotModel<T>> slots;


    #endregion

    /// <summary>
    /// Use this functionnality to initialize the inventory
    /// </summary>
    public void Init()
    {
        slots = new List<InventorySlotModel<T>>();

        for (int i = 0; i < numberOfSlot; i++)
        {
            slots.Add(new InventorySlotModel<T>(i, default(T), 0));
        }
    }

    /// <summary>
    /// Create a new slot
    /// </summary>
    public InventorySlotModel<T> CreateNewSlot()
    {
        numberOfSlot++;
        slots.Add(new InventorySlotModel<T>(numberOfSlot-1, default(T), 0));

        //Fire event
        if (!Equals(onSlotCreated, null))
            onSlotCreated(this, slots[numberOfSlot-1]);

        return slots[numberOfSlot - 1];

    }

    /// <summary>
    /// Return a slot if there is at least one slot available, othewise null
    /// </summary>
    /// <returns></returns>
    private InventorySlotModel<T> GetNextAvailableSlot()
    {
        foreach (InventorySlotModel<T> slot in slots)
        {
            if (slot.isAvailable)
                return slot;
        }

        return null;
    }

    /// <summary>
    /// Try to get a slot of the same item 
    /// </summary>
    /// <param name="pItem"></param>
    private InventorySlotModel<T> GetSlotToAddObject(T pObject)
    {
        InventorySlotModel<T> slotToReturn = null;

        foreach (InventorySlotModel<T> slot in slots)
        {
            if (slot.CanAddItem(pObject))
            {
                slotToReturn = slot;
                return slotToReturn;
            }
        }

        return slotToReturn;
    }

    /// <summary>
    /// Add the pItem into a slot if there is one slot available
    /// </summary>
    /// <param name="pItem"></param>
    /// <returns></returns>
    public bool AddObject(T pObject)
    {
        InventorySlotModel<T> slotWhereAddItem = GetSlotToAddObject(pObject);
        
        //If there is no slot
        if (slotWhereAddItem == null)
            return false;

        //Fire event
        if (!Equals(onObjectAdded, null))
            onObjectAdded(this, slotWhereAddItem);

        //handle if the add hasn't work
        return slotWhereAddItem.AddItem(pObject);

    }


    /// <summary>
    /// Get slot wich contains the specified item
    /// </summary>
    /// <param name="pItem"></param>
    /// <returns></returns>
    private InventorySlotModel<T> GetSlotWichContainsObject(T pObject)
    {
        foreach (InventorySlotModel<T> slot in slots)
        {
            if (slot.currentAmount > 0)
            {
                if (pObject.Compare(slot.objectReference))
                {
                    return slot;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Get slot wich contains the specified item
    /// </summary>
    /// <param name="pItem"></param>
    /// <returns></returns>
    private InventorySlotModel<T> GetSlotWichContainsObjectWithIdentfier(InventoryQueryIdentifier pIdentifier)
    {
        foreach (InventorySlotModel<T> slot in slots)
        {
            if (slot.currentAmount > 0)
            {
                if (slot.objectReference.CompareWithIdentifier(pIdentifier))
                {
                    return slot;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Return and remove the specified item within the inventory
    /// </summary>
    /// <param name="pItem"></param>
    /// <returns></returns>
    public T PullObject(T pObject)
    {
        InventorySlotModel<T> slot = GetSlotWichContainsObject(pObject);

        //If the inventory has no asked item
        if (slot == null)
            return default(T);

        //Fire event
        if (!Equals(onObjectRemoved, null))
            onObjectRemoved(this, slot);

        return slot.PullItem();
    }

    /// <summary>
    /// Returns the object reference of the same pObject
    /// </summary>
    /// <param name="pObject"></param>
    /// <returns></returns>
    public T GetObject(T pObject)
    {
        InventorySlotModel<T> slot = GetSlotWichContainsObject(pObject);

        //If the inventory has no asked item
        if (slot == null)
            return default(T);

        //Fire event
        if (!Equals(onObjectGot, null))
            onObjectGot(this, slot);

        return slot.objectReference;
    }

    /// <summary>
    /// Returns and remove the object in the specified slot
    /// </summary>
    /// <param name="pIndex"></param>
    /// <returns></returns>
    public T PullObjectAt(int pIndex)
    {
        //Fire event
        if (!Equals(onObjectRemoved, null))
            onObjectRemoved(this, slots[pIndex]);

        return slots[pIndex].PullItem();
    }

    /// <summary>
    /// Returns the object in the specified slot
    /// </summary>
    /// <param name="pIndex"></param>
    /// <returns></returns>
    public T GetObjectAt(int pIndex)
    {
        if (pIndex < numberOfSlot)
        {
            //Fire event
            if (!Equals(onObjectGot, null))
                onObjectGot(this, slots[pIndex]);

            return slots[pIndex].objectReference;
        }
        else
            return default(T);
    }

    /// <summary>
    /// Returns the pObject's index
    /// </summary>
    /// <param name="pObject"></param>
    /// <returns></returns>
    public int GetIndexObject(T pObject)
    {
        if (GetSlotWichContainsObject(pObject) != null)
            return GetSlotWichContainsObject(pObject).index;
        else
            return -1;
    }


    /// <summary>
    /// Returns and remove the object with the specified Identifier
    /// </summary>
    /// <param name="pIndex"></param>
    /// <returns></returns>
    public T PullObjectWithIdentifier(InventoryQueryIdentifier pIdentifier)
    {
        InventorySlotModel<T> slot = GetSlotWichContainsObjectWithIdentfier(pIdentifier);

        //If the inventory has no asked item
        if (slot == null)
            return default(T);

        //Fire event
        if (!Equals(onObjectRemoved, null))
            onObjectRemoved(this, slot);

        return slot.PullItem();
    }

    /// <summary>
    /// Returns the object with the specified Identifier
    /// </summary>
    /// <param name="pIndex"></param>
    /// <returns></returns>
    public T GetObjectWithIdentifier(InventoryQueryIdentifier pIdentifier)
    {
        InventorySlotModel<T> slot = GetSlotWichContainsObjectWithIdentfier(pIdentifier);

        //Fire event
        if (!Equals(onObjectAdded, null))
            onObjectGot(this, slot);
        if (slot != null)
            return slot.objectReference;
        else
            return default(T);
    }

    /// <summary>
    /// Replace in inventory pObject1 by pObject2.
    /// </summary>
    /// <param name="pObject1"></param>
    /// <param name="pObject2"></param>
    public void ReplaceObjectAt(T pObject1, T pObject2)
    {
        if (GetSlotWichContainsObject(pObject1) != null)
        {
            int _indexToReplace = GetSlotWichContainsObject(pObject1).index;

            slots[_indexToReplace] = new InventorySlotModel<T>(_indexToReplace, pObject2, 1);

            // Fire event only if there is subscribes
            if (!Equals(onObjectReplace, null))
                onObjectReplace(this, slots[_indexToReplace]);
        }
    }

    public void RemoveObject(T pObject)
    {
        InventorySlotModel<T> _slot = GetSlotWichContainsObject(pObject);

        _slot.ClearSlot();
    }
}
