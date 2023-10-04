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
        var users = this.users.GetOrAdd(documentName, _ => new Dictionary<long, ConnectedUser>());

        // We expect to have relatively few users per document, therefore we use normal lock here.
        lock (users)
        {
            if (users.TryGetValue(clientId, out var user))
            {
                var isChanged = false;

                if (clock > user.ClientClock)
                {
                    user.ClientClock = clock;
                    user.ClientState = state;
                    isChanged = true;
                }

                existingState = user.ClientState;

                // Always update the timestamp, because every call is an activity.
                user.LastActivity = Clock();

                return isChanged;
            }

            users.Add(clientId, new ConnectedUser
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
            // We expect to have relatively few users per document, therefore we use normal lock here.
            lock (users)
            {
                // Usually there should be nothing to remove, therefore we save a few allocations for the fast path.
                List<long>? usersToRemove = null;

                foreach (var (clientId, user) in users)
                {
                    if (user.LastActivity < olderThan)
                    {
                        usersToRemove ??= new List<long>();
                        usersToRemove.Add(clientId);

                        yield return (clientId, documentName);
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
}
