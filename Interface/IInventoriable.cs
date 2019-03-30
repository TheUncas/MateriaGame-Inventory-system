public interface IInventoriable
{
    /// <summary>
    /// Returns true if it's the same object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pObjectToCompare"></param>
    /// <returns></returns>
    bool Compare<T>(T pObjectToCompare);

    /// <summary>
    /// Return true if the it's the same object with the same Identifier
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pIdentifier"></param>
    /// <returns></returns>
    bool CompareWithIdentifier(InventoryQueryIdentifier pIdentifier);
}