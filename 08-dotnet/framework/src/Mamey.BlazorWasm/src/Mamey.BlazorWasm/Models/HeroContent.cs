namespace Mamey.BlazorWasm.Models;

public class HeroContent
{
    public string ImageSource { get; set; }
    public string Text { get; set; }
    public string SubText { get; set; }
    public int Top { get; set; } = 50;
    public int Left { get; set; } = 50;
    public int TranslateX = -50;
    public int TranslateY = -50;

}

public class FeaturedProduct()
{
    public int Id { get; set; } = default!;
    public string Title { get; set; }= string.Empty;
    public string Description { get; set; }= string.Empty;
    public string ImageUrl { get; set; }= string.Empty;
    public string ButtonHref { get; set; } = string.Empty;
    public string ButtonText { get; set; }= string.Empty;
}