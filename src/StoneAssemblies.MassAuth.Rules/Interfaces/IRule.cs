namespace StoneAssemblies.MassAuth.Rules.Interfaces
{
    using System.Threading.Tasks;

    public interface IRule<TMessage>
    {
        bool IsEnabled { get; }

        string Name { get; }

        int Priority { get; }

        Task<bool> EvaluateAsync(TMessage message);
    }
}