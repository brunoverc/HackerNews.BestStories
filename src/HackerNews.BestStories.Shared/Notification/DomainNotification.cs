namespace HackerNews.BestStories.Shared.Notification;

public class DomainNotification
{
    public DomainNotification(string key, string value)
    {
        DomainNotificationId = Guid.NewGuid();
        Version = 1;
        Key = key;
        Value = value;
    }

    public Guid DomainNotificationId { get; private set; }
    public string Key { get; private set; }
    public string Value { get; private set; }
    public int Version { get; private set; }
}