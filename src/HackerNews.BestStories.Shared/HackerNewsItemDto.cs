namespace HackerNews.BestStories.Shared;

public class HackerNewsItemDto
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string By { get; set; }
    public long Time { get; set; }
    public string Text { get; set; }
    public int Parent { get; set; }
    public int[] Kids { get; set; }
    public string Url { get; set; }
    public int Score { get; set; }
    public string Title { get; set; }
    public int Descendants { get; set; }
    
}