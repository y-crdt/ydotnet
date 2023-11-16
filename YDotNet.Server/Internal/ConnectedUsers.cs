using System.Collections.Concurrent;

namespace YDotNet.Server.Internal;

public sealed class ConnectedUsers
{
    private readonly ConcurrentDictionary<string, Dictionary<ulong, ConnectedUser>> users = new();

    public Func<DateTime> Clock = () => DateTime.UtcNow;

    public IReadOnlyDictionary<ulong, ConnectedUser> GetUsers(string documentName)
    {
        var documentUsers = users.GetOrAdd(documentName, _ => new Dictionary<ulong, ConnectedUser>());

        lock (documentUsers)
        {
            return new Dictionary<ulong, ConnectedUser>(documentUsers);
        }
    }

    public bool AddOrUpdate(string documentName, ulong clientId, ulong clock, string? state, out string? existingState)
    {
        var newUsers = this.users.GetOrAdd(documentName, _ => new Dictionary<ulong, ConnectedUser>());

        // We expect to have relatively few users per document, therefore we use normal lock here.
        lock (newUsers)
        {
            if (newUsers.TryGetValue(clientId, out var user))
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

            newUsers.Add(clientId, new ConnectedUser
            {
                ClientClock = clock,
                ClientState = state,
                LastActivity = Clock(),
            });

            existingState = state;
            return true;
        }
    }

    public bool Remove(string documentName, ulong clientId)
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

    public IEnumerable<(ulong ClientId, string DocumentName)> Cleanup(TimeSpan maxAge)
    {
        var olderThan = Clock() - maxAge;

        foreach (var (documentName, users) in users)
        {
            // We expect to have relatively few users per document, therefore we use normal lock here.
            lock (users)
            {
                // Usually there should be nothing to remove, therefore we save a few allocations for the fast path.
                List<ulong>? usersToRemove = null;

                foreach (var (clientId, user) in users)
                {
                    if (user.LastActivity < olderThan)
                    {
                        usersToRemove ??= new List<ulong>();
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
