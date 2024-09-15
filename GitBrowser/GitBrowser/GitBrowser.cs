namespace GitBrowser;

public class Program
{
	public static int Width = 1280;
	public static int Height = 720;

	public static void Main(string[] args)
	{
		var html = File.ReadAllText("C:\\Users\\thegi\\Documents\\GitHub\\jam001\\Site\\info.cern.ch\\hypertext\\WWW\\TheProject.html");
		var tokenizer = new Tokenizer(html);
		var tokens = tokenizer.Tokenize();

		var builder = new TreeBuilder(tokens);
		var domtree = builder.BuildTree();

		Console.ReadLine();
		return;
	}
}