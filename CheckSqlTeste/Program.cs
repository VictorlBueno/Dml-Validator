using SqlChecker;
using System;

class Program
{
    static void Main()
    {

        bool isValidSql = CheckSql.IsValid("INSERT INTO  tabela(teste,teste)VALUES ('ASQ',20);");

        Console.WriteLine(isValidSql);
    }
}
