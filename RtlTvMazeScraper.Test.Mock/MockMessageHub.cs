
namespace TvMazeScraper.Test.Mock
{
    using System.Threading.Tasks;
    using TvMazeScraper.Core.Interfaces;

    public class MockMessageHub : IMessageHub
    {
        public Task Publish<TMessage>(TMessage message)
        {
            return Task.CompletedTask;
        }
    }
}
