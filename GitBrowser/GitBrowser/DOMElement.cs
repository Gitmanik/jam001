namespace GitBrowser
{
	public class DOMElement
	{
		public required string Name;
		public required string? Value;
		public Dictionary<string, string> Attributes = [];
		public List<DOMElement> Children = [];
	}
}