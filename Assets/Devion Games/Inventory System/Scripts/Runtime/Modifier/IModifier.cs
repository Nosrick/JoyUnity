namespace DevionGames.InventorySystem
{
    public interface IModifier<T> 
    {
        void Modify(T item);
    }
}