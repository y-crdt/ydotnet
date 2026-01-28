using System.Collections.Concurrent;

namespace YDotNet.Server.Internal;

public sealed class ConnectedUsers
{
    private readonly ConcurrentDictionary<string, Dictionary<ulong, ConnectedUser>> users = new(StringComparer.Ordinal);

    public Func<DateTime> Clock { get; set; } = () => DateTime.UtcNow;

    public IReadOnlyDictionary<ulong, ConnectedUser> GetUsers(string documentName)
    {
        var documentUsers = users.GetOrAdd(documentName, _ => new Dictionary<ulong, ConnectedUser>());

        lock (documentUsers)
        {
            return new Dictionary<ulong, ConnectedUser>(documentUsers);
        }
    }

    public AddOrUpdateResult AddOrUpdate(string documentName, ulong clientId, ulong clock, string? state)
    {
        var newUsers = users.GetOrAdd(documentName, _ => new Dictionary<ulong, ConnectedUser>());

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

                // Always update the timestamp, because every call is an activity.
                user.LastActivity = Clock();

                return new AddOrUpdateResult(isChanged, IsNew: false, user.ClientState);
            }

            newUsers.Add(clientId, new ConnectedUser
            {
                ClientClock = clock,
                ClientState = state,
                LastActivity = Clock(),
            });

            return new AddOrUpdateResult(IsChanged: true, IsNew: true, state);
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

    public record AddOrUpdateResult(bool IsChanged, bool IsNew, string? ClientState);
}
