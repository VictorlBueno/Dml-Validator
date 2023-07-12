using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SqlChecker
{
    public class CheckSql
    {
        // Método para verificar se uma instrução SQL é válida
        public static bool IsValid(string sql)
        {
            bool result;
            sql = sql.Trim();
            string command = sql.Split(" ")[0].ToUpper();

            Console.ForegroundColor = ConsoleColor.Red;

            switch (command)
            {
                case "INSERT":
                    result = IsInsertValid(sql);
                    break;
                case "DELETE":
                    result = IsInsertValid(sql);
                    break;
                case "UPDATE":
                    result = IsInsertValid(sql);
                    break;
                default:
                    result = false;
                    Console.WriteLine($"Syntax error: Command '{command}' is not valid.");
                    break;
            }

            Console.ResetColor();
            return result;
        }

        // Método para verificar se uma instrução INSERT é válida
        public static bool IsInsertValid(string sql)
        {
            List<string> sqlAsList = sql.Split(" ").ToList();
            sqlAsList.RemoveAll(" ".Contains);
            string sqlWithNoSpaces = sql.Replace(" ", "");
            Regex regex = new Regex("^[a-zA-Z0-9\\s]+$");

            // Verificar se a sintaxe INSERT contém INTO imediatamente após
            if (!sqlAsList[1].Equals("INTO"))
            {
                Console.WriteLine($"Syntax error: '{sqlAsList[0]} {sqlAsList[1]}' is not valid.");
                return false;
            }
            // Procurar por caracteres especiais no nome da tabela
            else if (!regex.IsMatch(sqlAsList[2].Split("(")[0]))
            {
                Console.WriteLine($"Syntax error: Table name '{sqlAsList[2].Split("(")[0]}' contains special characters.");
                return false;
            }
            // Procurar por parênteses correspondentes
            else if (!sql.Contains('(') || !sql.Contains(')'))
            {
                Console.WriteLine($"Syntax error: Parentheses not found.");
                return false;
            }
            // Verificar se existe parênteses ao redor dos nomes das colunas
            if (sqlWithNoSpaces[10 + sqlAsList[2].Split("(")[0].Length]
                                                    .ToString()
                                                    .Equals("(") && sqlWithNoSpaces[sqlWithNoSpaces
                                                    .IndexOf("VALUES") - 1]
                                                    .ToString()
                                                    .Equals(")"))
            {
                // Obter os nomes das colunas e a quantidade
                int openParenthesesIndex = sql.IndexOf('(');
                string columnNames = sql.Substring(openParenthesesIndex + 1, sql.IndexOf(')') - openParenthesesIndex - 1);
                List<string> columnNamesList = columnNames.Trim()
                                                            .Split(",")
                                                            .ToList();
                int numOfCols = columnNamesList.Count;

                // Verificar se há vírgulas faltando/extras entre os nomes das colunas
                if (columnNamesList.Count(a => a.Trim().Contains(" ") || string.IsNullOrEmpty(a)) > 0)
                {
                    Console.WriteLine("Syntax error: Missing/extra commas between column names.");
                    return false;
                }
                // Verificar se há caracteres especiais nos nomes das colunas
                else if (!regex.IsMatch(columnNames.Replace(",", "")))
                {
                    Console.WriteLine($"Syntax error: Column name '{columnNames}' is not valid.");
                    return false;
                }

                // Verificar se a sintaxe "VALUES" está presente
                int closeParenthesesIndex = sql.IndexOf(')');
                string getValueString = sql.Substring(closeParenthesesIndex + 1, sql.IndexOf('(', closeParenthesesIndex) - closeParenthesesIndex - 1).Replace(" ", "");

                if (!getValueString.Equals("VALUES"))
                {
                    Console.WriteLine($"Syntax error: '{getValueString}' should be 'VALUES'.");
                    return false;
                }
            }
            // Se as colunas não estiverem presentes, a próxima palavra deve ser "VALUES"
            else if (!sqlAsList[3].Contains("VALUES"))
            {
                Console.WriteLine($"Syntax error: Parentheses of the columns not found.");
                return false;
            }
            // Se os nomes das colunas não estiverem presentes, verificar se há a inicialização ( de valores
            else if (!sqlWithNoSpaces[sqlWithNoSpaces.IndexOf("VALUES") + 6].ToString().Equals("("))
            {
                Console.WriteLine($"Syntax error: Parentheses not found.");
                return false;
            }

            // Verificar se o parêntese de valores foi fechado
            string sqlWithNoSpacesAndSemicolon = sqlWithNoSpaces.Replace(";", "");
            if (!sqlWithNoSpacesAndSemicolon[^1].Equals(')'))
            {
                Console.WriteLine($"Syntax error: Parentheses not found.");
                return false;
            }

            // FALTA VER OS VALORES DENTRO DO ()
            if(sql.IndexOf("'") != -1 && sql.IndexOf('"') != -1)
            {
                bool typeOfComma = sql.IndexOf("'") > sql.IndexOf('"') ? true : false;
            }

            string insertValues = sql.Substring(sql.IndexOf("VALUES") + 6);

            // Verificar outras regras de validação para a instrução INSERT...

            return true;
        }
    }
}
