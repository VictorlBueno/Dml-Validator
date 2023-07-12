using SqlChecker;
using System;

class Program
{
    static void Main()
    {
        SqlChecker.CheckSql checkSql = new();
        string teste = "  coluna,s,  ss    coluna2, coluna2,   ";

        List<string> columnNamesListComma = teste.Trim()
                                                    .Split(",")
                                                    .ToList();

        foreach (string columnName in columnNamesListComma)
        {
            Console.WriteLine(columnName);
        }
        Console.WriteLine(columnNamesListComma.Count);
        Console.WriteLine(columnNamesListComma.Count(a => a.Trim().Contains(" ") || string.IsNullOrEmpty(a)));
    }
}
