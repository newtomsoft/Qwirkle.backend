using System.Threading.Tasks;

namespace Qwirkle.Core.Ports;

public interface IAuthentication
{
    Task<bool> RegisterAsync(User user, string password);
}