namespace JoyLib.Code.Entities.Sexuality
{
    public interface IEntitySexualityHandler
    {
        ISexuality Get(string sexuality);
        ISexuality[] Sexualities { get; }
    }
}