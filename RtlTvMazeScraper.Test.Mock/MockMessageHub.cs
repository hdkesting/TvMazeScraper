
namespace RtlTvMazeScraper.Test.Mock
{
    using System.Threading.Tasks;
    using RtlTvMazeScraper.Core.Interfaces;

    public class MockMessageHub : IMessageHub
    {
        public Task Publish<TMessage>(TMessage message)
        {
            return Task.CompletedTask;
        }
    }
}
