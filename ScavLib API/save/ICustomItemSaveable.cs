namespace ScavLib.save
{

    public interface ICustomItemSaveable
    {

        string Save();

        void Load(string blob);
    }
}
