namespace StoneAssemblies.MassAuth.Hosting.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    using MassTransit;

    using Microsoft.Extensions.Hosting;

    public sealed class BusHostedService : IHostedService
    {
        private readonly IBusControl _busControl;

        public BusHostedService(IBusControl busControl)
        {
            this._busControl = busControl;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return this._busControl.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return this._busControl.StopAsync(cancellationToken);
        }
    }
}