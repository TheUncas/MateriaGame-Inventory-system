﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#region Delegates
public delegate void InventorySlotModelEventHandler<T>(InventorySlotModel<T> pInventorySlot) where T : IInventoriable;
#endregion

/// <summary>
/// Class wich represents a slot of an inventory
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public class InventorySlotModel<T> where T : IInventoriable
{

    #region events
    public InventorySlotModelEventHandler<T> onSlotUpdated;
    #endregion

    #region Properties

    /// <summary>
    /// Id of the slot
    /// </summary>
    public int index;

    /// <summary>
    /// This value is setup when the item is stored in the slot for the first time
    /// </summary>
    [SerializeField]
    private int allowedAmountInSlot = 1;

    /// <summary>
    /// Current number of item;
    /// </summary>
    public int currentAmount;

    /// <summary>
    /// Flag that indicates if the slot is available
    /// </summary>
    public bool isAvailable;

    /// <summary>
    /// Object stored in this slot
    /// </summary>
    public T objectReference;

    #endregion

    #region Constructor
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pItems"></param>
    public InventorySlotModel(int pIndex, T pObject, int pNumberOfObject)
    {
        index = pIndex;

        //Initialize the slot
        ClearSlot();

        //Add items into the list
        AddItems(pObject, pNumberOfObject);
    }

    #endregion

    #region Implementation

    /// <summary>
    /// Clear the inventory slot
    /// </summary>
    public void ClearSlot()
    {
        isAvailable = true;
        currentAmount = 0;
        OnSlotUpdated();
    }

    public void SetMaximumAmountInSlot(int pMaximumAmount)
    {
        allowedAmountInSlot = pMaximumAmount;
    }

    /// <summary>
    /// Returns true if the item has been added
    /// </summary>
    /// <returns></returns>
    public bool AddItem(T pObject)
    {
        if (CanAddItem(pObject))
        {
            objectReference = pObject;
            currentAmount++;
            isAvailable = false;

            OnSlotUpdated();

            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Returns true if we can add the item, false otherwise
    /// </summary>
    /// <param name="pItem"></param>
    /// <returns></returns>
    public bool CanAddItem(T pObject)
    {
        if (objectReference == null)
            return true;

        //No item on the slot
        if (isAvailable || currentAmount == 0)
            return true;

        //If it's not the same item
        if (!pObject.Compare(objectReference))
            return false;

        //If there is no more space
        if (currentAmount >= allowedAmountInSlot)
            return false;

        return true;
    }

    /// <summary>
    /// Add items into the slot. If the amount exceed the allowed number, returns items left
    /// </summary>
    public int AddItems(T pObject, int pNumberOfItemToAdd)
    {
        int numberOfItemLeft = pNumberOfItemToAdd;

        if (pObject != null)
        {
            if (CanAddItem(pObject))
            {
                //Number of item leaving if we can't add everything
                numberOfItemLeft = pNumberOfItemToAdd - (allowedAmountInSlot - currentAmount);

                //If we added everything
                if (numberOfItemLeft < 0)
                {
                    numberOfItemLeft = 0;
                    currentAmount += pNumberOfItemToAdd;
                }
                else
                {
                    currentAmount += pNumberOfItemToAdd - numberOfItemLeft;
                }
            }

            OnSlotUpdated();
        }

        return numberOfItemLeft;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pItem"></param>
    /// <returns></returns>
    public T PullItem()
    {

        //If no item on the slot
        if (currentAmount < 0)
            return default(T);

        //If there is one item exactly
        if (currentAmount == 1)
        {
            ClearSlot();
            OnSlotUpdated();
            return objectReference;
        }

        //If there is more than 1 item
        if (currentAmount > 1)
        {
            currentAmount--;
            OnSlotUpdated();
            return objectReference;
        }


        //Other situation not handled yet
        return default(T);
    }

    #endregion

    #region Events trigger
    /// <summary>
    /// Trigger event onSlotUpdated
    /// </summary>
    public void OnSlotUpdated()
    {
        if (onSlotUpdated != null)
            onSlotUpdated(this);
    }
    #endregion

}
