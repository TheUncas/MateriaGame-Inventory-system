/// <summary>
/// Classed designed to be used for querying the inventory
/// </summary>
/// <typeparam name="T"></typeparam>
public class InventoryQueryIdentifier
{

    #region Properties

    /// <summary>
    /// represent the identifier for a object within the inventory
    /// </summary>
    public object objectIdentifier;

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pObjectIdentifier"></param>
    public InventoryQueryIdentifier(object pObjectIdentifier)
    {
        objectIdentifier = pObjectIdentifier;
    }

    #endregion

}
