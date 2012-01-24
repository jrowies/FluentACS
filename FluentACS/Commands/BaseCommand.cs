namespace FluentACS.Commands
{
    using System;

    using FluentACS.Logging;

    public abstract class BaseCommand : ICommand
    {
        public void Execute(object receiver)
        {
            this.Execute(receiver, null);
        }

        public abstract void Execute(object receiver, Action<LogInfo> logAction);

        public void LogMessage(Action<LogInfo> logAction, string message)
        {
            if (logAction != null)
            {
                logAction(new LogInfo { LogInfoType = LogInfoTypeEnum.Information, Message = message });
            }
        }

        public void LogSavingChangesMessage(Action<LogInfo> logAction)
        {
            if (logAction != null)
            {
                logAction(new LogInfo { LogInfoType = LogInfoTypeEnum.Information, Message = "Saving Changes... done" });
            }
        }
    }
}