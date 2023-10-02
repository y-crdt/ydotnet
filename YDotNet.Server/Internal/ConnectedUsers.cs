using System.Collections.Concurrent;

namespace YDotNet.Server.Internal;

public sealed class ConnectedUsers
{
    private readonly ConcurrentDictionary<string, Dictionary<long, ConnectedUser>> users = new();

    public Func<DateTime> Clock = () => DateTime.UtcNow;

    public IReadOnlyDictionary<long, ConnectedUser> GetUsers(string documentName)
    {
        var documentUsers = users.GetOrAdd(documentName, _ => new Dictionary<long, ConnectedUser>());

        lock (documentUsers)
        {
            return new Dictionary<long, ConnectedUser>(documentUsers);
        }
    }

    public bool AddOrUpdate(string documentName, long clientId, long clock, string? state, out string? existingState)
    {
        var documentUsers = users.GetOrAdd(documentName, _ => new Dictionary<long, ConnectedUser>());

        lock (documentUsers)
        {
            if (documentUsers.TryGetValue(clientId, out var user))
            {
                var isChanged = false;

                if (clock > user.ClientClock)
                {
                    user.ClientClock = clock;
                    user.ClientState = state;
                }

                existingState = user.ClientState;

                user.LastActivity = Clock();
                return isChanged;
            }

            documentUsers.Add(clientId, new ConnectedUser
            {
                ClientClock = clock,
                ClientState = state,
                LastActivity = Clock(),
            });

            existingState = state;
            return true;
        }
    }

    public bool Remove(string documentName, long clientId)
    {
        if (!users.TryGetValue(documentName, out var documentUsers))
        {
            return false;
        }

        lock (documentUsers)
        {
            return documentUsers.Remove(clientId);
        }
    }
    
    public IEnumerable<(long ClientId, string DocumentName)> Cleanup(TimeSpan maxAge)
    {
        var olderThan = Clock() - maxAge;

        foreach (var (documentName, users) in users)
        {
            List<long>? usersToRemove = null;

            lock (users)
            {
                foreach (var (clientId, user) in users)
                {
                    if (user.LastActivity < olderThan)
                    {
                        usersToRemove ??= new List<long>();
                        usersToRemove.Add(clientId);

                        yield return (clientId, documentName);
                    }
                }
            }

            if (usersToRemove != null)
            {
                foreach (var user in usersToRemove)
                {
                    users.Remove(user);
                }
            }
        }
    }
}
