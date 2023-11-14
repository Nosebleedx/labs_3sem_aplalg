using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;

string filePath = "C:\\Users\\Дмитрий\\source\\repos\\ThirdLabPA\\rules.csv";
CSVReader csvReader = new CSVReader(filePath);
Dictionary<string, Dictionary<string, string>> transitions = csvReader.ReadCSV();
List<string> admVert = new List<string>();

var minimizedDict = Minimizer.Minimization(transitions, admVert);


foreach (var outerKey in minimizedDict.Keys)
{
    Console.WriteLine("Outer Key: " + outerKey);
    foreach (var innerKey in minimizedDict[outerKey].Keys)
    {
        Console.WriteLine("Inner Key: " + innerKey + ", Value: " + minimizedDict[outerKey][innerKey]);
    }
}
public class CSVReader
{
    private readonly string _filePath;

    public CSVReader(string filePath)
    {
        _filePath = filePath;
    }

    public Dictionary<string, Dictionary<string, string>> ReadCSV()
    {
        Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();

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
                        string symbol = parts[1][0].ToString();
                        string nextState = parts[2];

                        if (!dictionary.ContainsKey(key))
                        {
                            dictionary[key] = new Dictionary<string, string>();
                        }

                        dictionary[key].Add(symbol, nextState);
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

public class Minimizer
{
    private Dictionary<string, Dictionary<string, string>> My_dict;
    private List<string> AdmittedVertexes;
    private static Dictionary<string, Dictionary<string, int>> Marked_Matrix;
    public static string StartVertex = "a";
    public Minimizer(Dictionary<string, Dictionary<string, string>> My_dict, List<string> amdVert)
    {
        this.My_dict = My_dict;
        this.AdmittedVertexes = amdVert;
    }

    public static Dictionary<string, Dictionary<string, string>> Minimization(Dictionary<string, Dictionary<string, string>> My_dict, List<string> amdVert)
    {

        var marked_matrix = createStartMatrix(My_dict);
        var AdmittedVertexes = createAdmVert(My_dict);
        /*foreach (var outerKey in My_dict.Keys)
        {
            Console.WriteLine("Outer Key: " + outerKey);
            foreach (var innerKey in My_dict[outerKey].Keys)
            {
                Console.WriteLine("Inner Key: " + innerKey + ", Value: "  + My_dict[outerKey][innerKey]);
            }
        }*/


        bool flag = true;
        while (flag)
        {
            string[] vars;
            foreach (string i in marked_matrix.Keys)
            {
                foreach (string j in marked_matrix[i].Keys)
                {
                    if (marked_matrix[i][j] == 0)
                    {
                        foreach (string w in My_dict[i].Keys)
                        {
                            try
                            {

                                vars = new String[]{
                                        My_dict[i][w],
                                        My_dict[j][w]
                                };
                                /* Console.WriteLine($"{vars[0]} EQUALS WITH {vars[1]}");*/
                                if ((!vars[0].Equals(vars[1]))
                                            && (marked_matrix[vars[0]][vars[1]] == 1
                                            || (AdmittedVertexes.Contains(vars[0])
                                            && !AdmittedVertexes.Contains(vars[1]))
                                            || (AdmittedVertexes.Contains(vars[1])
                                            && !AdmittedVertexes.Contains(vars[0]))))
                                {
                                    marked_matrix[i][j] = 1;
                                    marked_matrix[j][i] = 1;
                                    flag = false;
                                    //Console.WriteLine("OTMETIL");
                                }
                            }
                            catch (Exception e) { Console.WriteLine("Exeption!!! - - - " + e.Message); }
                        }
                    }
                }
            }

            if (!flag)
            {
                flag = true;
            }
            else
            {
                flag = false;
            }
        }
        /*foreach (var outerKey in marked_matrix.Keys)
        {
            Console.WriteLine("Outer Key: " + outerKey);
            foreach (var innerKey in marked_matrix[outerKey].Keys)
            {
                Console.WriteLine("Inner Key: " + innerKey + ", Value: " + marked_matrix[outerKey][innerKey]);
            }
        }*/
        var minimizedDict = createMinimizedDict(marked_matrix, My_dict);
        return minimizedDict;
    }

    public static List<string> createAdmVert(Dictionary<string, Dictionary<string, string>> dict)
    {
        List<string> lst = dict.Keys.Where(key => key.ToUpper() != "C").ToList();
        return lst;
    }
    public static Dictionary<string, Dictionary<string, int>> createStartMatrix(Dictionary<string, Dictionary<string, string>> start_dict)
    {
        Dictionary<string, Dictionary<string, int>> Zeros_Matrix = start_dict.ToDictionary(
            outerDict => outerDict.Key,
            outerDict => start_dict
                                   .Where(innerDict => innerDict.Key != outerDict.Key)
                                   .ToDictionary(innerDict => innerDict.Key, innerDict => 0)
            );
        return Zeros_Matrix;
    }

    public static Dictionary<string, Dictionary<string, string>> createMinimizedDict(Dictionary<string, Dictionary<string, int>> matrix, Dictionary<string, Dictionary<string,string>> currentAutomat)
    {
        Dictionary<string, Dictionary<string, string>> minimizedDict = new Dictionary<string, Dictionary<string, string>>();

        string[] columns = matrix[StartVertex].Keys.ToArray();
        Dictionary<string, Dictionary<string, string>> optimizedAutomate = new Dictionary<string, Dictionary<string, string>>();
        HashSet<string> usageVertexes = new HashSet<string>();
        string[] variants;

        foreach (var v in matrix.Keys)
        {
            foreach (var vv in matrix[v].Keys)
            {
                // Console.WriteLine();
                if (matrix[v][vv] == 0 && matrix[v].ContainsKey(vv))
                {
                    if (!optimizedAutomate.ContainsKey(v + vv) && !optimizedAutomate.ContainsKey(vv + v))
                    {
                        // Console.WriteLine(v + vv, "ETO SOEDINENNAYA V + VV");
                        optimizedAutomate[v + vv] = new Dictionary<string, string>();
                        usageVertexes.Add(v);
                        usageVertexes.Add(vv);
                        /*foreach (var outerKey in matrix.Keys)
                        {
                            Console.WriteLine("Outer Key: " + outerKey);
                            foreach (var innerKey in matrix[outerKey].Keys)
                            {
                                Console.WriteLine("Inner Key: " + innerKey + ", Value: " + matrix[outerKey][innerKey]);
                            }
                        }
                        */
                        foreach (string way in columns)
                        {
                            if (!v.Equals(way) && !vv.Equals(way))
                            {
                                // Console.WriteLine(v, "AND", way);
                                variants = new string[2]
                                {
                                    matrix[v][way].ToString(),
                                    matrix[vv][way].ToString()
                                };

                                if (variants[0] == variants[1])
                                {
                                    optimizedAutomate[v + vv][way] = variants[0];
                                }
                                else
                                {
                                    optimizedAutomate[v + vv][way] = variants[0] + variants[1];
                                }
                            }
                        }
                    }
                }
            }
        }

        foreach (string v in matrix.Keys)
        {
            if (!usageVertexes.Contains(v))
            {
                optimizedAutomate[v] = new Dictionary<string, string>(currentAutomat[v].ToDictionary(pair => pair.Key, pair => pair.Value));
            }
            else
            {
                foreach (string vv in optimizedAutomate.Keys)
                {
                    if(vv.Length == 2)
                    {
                        optimizedAutomate[vv] = new Dictionary<string, string>(currentAutomat[v].ToDictionary(pair => pair.Key, pair => pair.Value));                        
                    }
                }
            }
        }
        foreach (string v in optimizedAutomate.Keys)
        {
            if (v.Length == 2)
            {
                foreach (string externalKey in optimizedAutomate.Keys)
                {
                    foreach (var transition in optimizedAutomate[externalKey])
                    {
                        if (usageVertexes.Contains(transition.Value) && v.Contains(transition.Value))
                        {
                            optimizedAutomate[externalKey][transition.Key] = v;
                        }
                    }
                }
            }
        }
        return optimizedAutomate;
    }
}



