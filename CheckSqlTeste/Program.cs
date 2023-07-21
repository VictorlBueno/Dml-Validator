using SqlChecker;
using System;

class Program
{
    static void Main()
    {

        bool isValidSql = CheckSql.IsValid("INSERT INTO  tabela VALUES (23,20);");

        Console.WriteLine(isValidSql);
    }
}
