using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

string filePath = "C:\\Users\\Дмитрий\\source\\repos\\2лаба_ПА_НКА\\rules.csv";
CSVReader csvReader = new CSVReader(filePath);
//foreach (var kvp in result)
//{
//	Console.WriteLine($"Состояние: {kvp.Key}");
//	Console.WriteLine("Переходы:");

//	foreach (var transition in kvp.Value)
//	{
//		Console.WriteLine($"  По символу '{transition.Item1}' переходит в состояние '{transition.Item2}'");
//	}

//	Console.WriteLine();
//}
Dictionary<string, List<Tuple<char, string>>> result = csvReader.ReadCSV();

NFA example = new NFA(result, "a");

string input = "0011111";
List<string> acceptedChains = example.IsAccepted(input);

if (acceptedChains.Count > 0)
{
	Console.WriteLine($"Цепочка '{input}' подходит.");
	Console.WriteLine("Успешно пройденные цепи:");

	foreach (string chain in acceptedChains)
	{
		Console.WriteLine(chain);
	}
}
else
{
	Console.WriteLine($"Цепочка '{input}' не подходит.");
}

public class CSVReader
{
	private readonly string _filePath;

	public CSVReader(string filePath)
	{
		_filePath = filePath;
	}

	public Dictionary<string, List<Tuple<char, string>>> ReadCSV()
	{
		Dictionary<string, List<Tuple<char, string>>> dictionary = new Dictionary<string, List<Tuple<char, string>>>();

		try
		{
			using (var reader = new StreamReader(_filePath))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					string[] parts = line.Split(',');

					if (parts.Length >= 3)
					{
						string key = parts[0];
						char symbol = parts[1][0];
						string nextState = parts[2];

						if (!dictionary.ContainsKey(key))
						{
							dictionary[key] = new List<Tuple<char, string>>();
						}

						dictionary[key].Add(Tuple.Create(symbol, nextState));
					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ошибка при чтении CSV-файла: {ex.Message}");
		}

		return dictionary;
	}
}

public class NFA
{
	private readonly Dictionary<string, List<Tuple<char, string>>> transitions;
	private readonly string startState;

	public NFA(Dictionary<string, List<Tuple<char, string>>> transitions, string startState)
	{
		this.transitions = transitions;
		this.startState = startState;
	}

	public List<string> IsAccepted(string input)
	{
		List<string> currentStates = new List<string> { startState };
		List<string> acceptedChains = new List<string>();

		foreach (char symbol in input)
		{
			List<string> nextStates = new List<string>();

			foreach (string state in currentStates)
			{
				if (transitions.ContainsKey(state))
				{
					foreach (var transition in transitions[state])
					{
						if (transition.Item1 == symbol)
						{
							nextStates.Add(transition.Item2);
							Console.WriteLine($"Переход из '{state}' в '{transition.Item2}' по символу '{symbol}'");
						}
					}
				}
			}

			currentStates = nextStates.Distinct().ToList();

			if (currentStates.Count == 0)
			{
				// Цепочка встала в тупик
				return acceptedChains;
			}
		}

		// Если дошли до конца входной цепочки, то она успешна
		if (currentStates.Count > 0)
		{
			acceptedChains.Add(input);
		}

		return acceptedChains;
	}
}