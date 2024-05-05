using TaktTusur.Media.Core.Interfaces;

namespace TaktTusur.Media.Infrastructure.Services;

public class TextTransformer : ITextTransformer
{
	public string MakeShorter(string text, int maxLength = 0, int maxParagraphs = 0)
	{
		if (text.Length < maxLength) return text;
		
		if (maxLength > 0 && text.Length > maxLength)
		{
			var lastSpaceIndex = text.LastIndexOf(' ', maxLength);

			text = text.Substring(0, lastSpaceIndex > 0 ? lastSpaceIndex : maxLength);
		}

		if (maxParagraphs <= 0) return text;
		var paragraphs = text.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
		if (paragraphs.Length > maxParagraphs)
		{
			text = string.Join("\r\n\r\n", paragraphs.Take(maxParagraphs));
		}

		return text;
	}
}