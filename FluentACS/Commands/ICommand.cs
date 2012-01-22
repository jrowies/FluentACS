namespace FluentACS.Commands
{
    public interface ICommand
    {
        void Execute(object receiver);
    }
}