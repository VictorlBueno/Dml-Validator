using SqlChecker;
using System;

class Program
{
    static void Main()
    {

        bool isValidSql = CheckSql.IsValid("INSERT INTO tabela(teste, tesd) VALUES('ASQ',20)");

        Console.WriteLine(isValidSql);
    }
}
