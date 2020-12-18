namespace JoyLib.Code.Rollers
{
    public interface IRollable
    {
        int Roll(int lower, int upper);

        int RollSuccesses(int number, int threshold);
    }
}
