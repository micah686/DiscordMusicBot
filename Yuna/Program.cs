global using Victoria.Node;
global using Victoria.Player;
global using Yuna.Services.Player;

using System.Threading.Tasks;
using Yuna.Services;

namespace Yuna
{
    class Program
    {
        static void Main(string[] args)
        => new Bot().MainAsync().GetAwaiter().GetResult();
    }
}
