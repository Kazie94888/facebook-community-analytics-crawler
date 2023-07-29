using System.Threading.Tasks;

namespace LookOn.Jobs.Jobs
{
    public interface IIntegrationJob
    {
        Task Execute();
    }

    public abstract class BackgroundJobBase : LookOnManager, IIntegrationJob
    {
        public virtual Task Execute()
        { 
           return DoExecute();
        }

        protected abstract Task DoExecute();
    }
}