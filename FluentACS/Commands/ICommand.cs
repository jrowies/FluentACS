namespace FluentACS.Commands
{
    using System;

    using FluentACS.Logging;

    public interface ICommand
    {
        void Execute(object receiver, Action<LogInfo> logAction);

        void Execute(object receiver);
    }
}