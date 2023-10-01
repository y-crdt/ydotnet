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
            return documentUsers.ToDictionary(
                x => x.Key,
                x => new ConnectedUser
                {
                    State = new Dictionary<string, object>(x.Value.State)
                });
        }
    }

    public bool Add(string documentName, long clientId)
    {
        var documentUsers = users.GetOrAdd(documentName, _ => new Dictionary<long, ConnectedUser>());

        lock (documentUsers)
        {
            if (documentUsers.ContainsKey(clientId))
            {
                return false;
            }

            documentUsers.Add(clientId, new ConnectedUser
            {
                LastActivity = Clock(),
            });

            return true;
        }
    }

    public ConnectedUser SetAwareness(string documentName, long clientId, string key, object value)
    {
        var documentUsers = users.GetOrAdd(documentName, _ => new Dictionary<long, ConnectedUser>());

        ConnectedUser user;

        lock (documentUsers)
        {
            if (!documentUsers.TryGetValue(clientId, out user!))
            {
                user = new ConnectedUser
                {
                    LastActivity = Clock(),
                };

                documentUsers.Add(clientId, user);
            }
        }

        lock (user)
        {
            user.State[key] = value;
        }

        return user;
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
