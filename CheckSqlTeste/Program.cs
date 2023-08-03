using SqlChecker;
using System;

class Program
{
    static void Main()
    {

        Console.WriteLine(CheckDml.IsValid("INSERT INTO  tabela(as,ds) VALUES (200.03,20);"));

    }
}
