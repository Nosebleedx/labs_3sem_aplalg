using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

string filePath = "C:\\Users\\Дмитрий\\source\\repos\\ПА_1лаба_ДКА\\rules.csv";
CSVReader csvReader = new CSVReader(filePath);
Dictionary<string, string[]> result = csvReader.ReadCSV();

int[] sequence = { 0, 0, 1 };
int[] second_sequence = { 0, 1, 0, 0, 0, 0, 0 };
DFA example = new DFA(result, second_sequence, "A");

example.Sequence_check();


public class CSVReader
{
	private readonly string _filePath;

	public CSVReader(string filePath)
	{
		_filePath = filePath;
	}

	public Dictionary<string, string[]> ReadCSV()
	{
		Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();

		try
		{
			using (var reader = new StreamReader(_filePath))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					string[] parts = line.Split(',');

					if (parts.Length > 1)
					{
						string key = parts[0];
						string[] values = new string[2];

						values[0] = parts.Length > 1 ? parts[1] : null;
						values[1] = parts.Length > 2 ? parts[2] : null;

						dictionary[key] = values;
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


class DFA
{
	Dictionary<string, string[]> rules = new Dictionary<string, string[]>();
	int[] Sequence { get; set; }
	string start_state { get; set; }


	public DFA(Dictionary<string, string[]> rules, int[] sequence, string main_state)
	{
		this.rules = rules;
		Sequence = sequence;
		this.start_state = main_state;
	}
	public bool Sequence_check()
	{
		string current_state = start_state;

		foreach (int s in Sequence)
		{
			if (!string.IsNullOrEmpty(rules[current_state][s]))
			{
				current_state = rules[current_state][s];
				Console.WriteLine("ЕСТЬ ПУТЬ");
				Console.WriteLine($"Я сейчас тут: {current_state}");
			}
			else
			{
				Console.WriteLine("Последовательность плохая");
				return false;
			}
		}
        Console.WriteLine("Последовательность хорошая");
        return true;
	}
}


