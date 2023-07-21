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

            // Define a cor de destaque para mensagens de erro
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
                    // Exibe mensagem de erro para comandos inválidos
                    Console.WriteLine($"Syntax error: Command '{command}' is not valid.");
                    break;
            }

            // Reseta a cor da fonte para o padrão
            Console.ResetColor();
            return result;
        }

        // Método para verificar se uma instrução INSERT/DELETE/UPDATE é válida
        private static bool IsInsertValid(string sql)
        {
            List<string> sqlAsList = sql.Split(" ").ToList();
            sqlAsList.RemoveAll(" ".Contains);
            string sqlWithNoSpaces = sql.Replace(" ", "");
            Regex regex = new Regex("^[a-zA-Z0-9\\s]+$");

            // Verifica se a sintaxe INSERT contém INTO imediatamente após
            if (!sqlAsList[1].Equals("INTO"))
            {
                // Exibe mensagem de erro para o caso de INTO estar ausente
                Console.WriteLine($"Syntax error: '{sqlAsList[0]} {sqlAsList[1]}' is not valid.");
                return false;
            }
            // Procura por caracteres especiais no nome da tabela
            else if (!regex.IsMatch(sqlAsList[2].Split("(")[0]))
            {
                // Exibe mensagem de erro se o nome da tabela contiver caracteres especiais
                Console.WriteLine($"Syntax error: Table name '{sqlAsList[2].Split("(")[0]}' contains special characters.");
                return false;
            }
            // Procura por parênteses correspondentes
            else if (!sql.Contains('(') || !sql.Contains(')'))
            {
                // Exibe mensagem de erro se os parênteses estiverem ausentes
                Console.WriteLine($"Syntax error: Parentheses not found.");
                return false;
            }

            // Verifica se existe parênteses ao redor dos nomes das colunas
            int numOfCols = 0;
            if (sqlWithNoSpaces[10 + sqlAsList[2].Split("(")[0].Length]
                                .ToString()
                                .Equals("(") && sqlWithNoSpaces[sqlWithNoSpaces
                                .IndexOf("VALUES") - 1]
                                .ToString()
                                .Equals(")"))
            {
                // Obtém os nomes das colunas e a quantidade
                int openParenthesesIndex = sql.IndexOf('(');
                string columnNames = sql.Substring(openParenthesesIndex + 1, sql.IndexOf(')') - openParenthesesIndex - 1);
                List<string> columnNamesList = columnNames.Trim()
                                                            .Split(",")
                                                            .ToList();
                numOfCols = columnNamesList.Count;

                // Verifica se há vírgulas faltando/extras entre os nomes das colunas
                if (columnNamesList.Count(a => a.Trim().Contains(" ") || string.IsNullOrEmpty(a)) > 0)
                {
                    // Exibe mensagem de erro se houver problemas com as vírgulas entre os nomes das colunas
                    Console.WriteLine("Syntax error: Missing/extra commas between column names.");
                    return false;
                }
                // Verifica se há caracteres especiais nos nomes das colunas
                else if (!regex.IsMatch(columnNames.Replace(",", "")))
                {
                    // Exibe mensagem de erro se o nome das colunas contiver caracteres especiais
                    Console.WriteLine($"Syntax error: Column name '{columnNames}' is not valid.");
                    return false;
                }

                // Verifica se a sintaxe "VALUES" está presente
                int closeParenthesesIndex = sql.IndexOf(')');
                string getValueString = sql.Substring(closeParenthesesIndex + 1, sql.IndexOf('(', closeParenthesesIndex) - closeParenthesesIndex - 1).Replace(" ", "");

                if (!getValueString.Equals("VALUES"))
                {
                    // Exibe mensagem de erro se "VALUES" não estiver presente
                    Console.WriteLine($"Syntax error: '{getValueString}' should be 'VALUES'.");
                    return false;
                }
            }
            // Se as colunas não estiverem presentes, a próxima palavra deve ser "VALUES"
            else if (!sqlAsList[3].Contains("VALUES"))
            {
                // Exibe mensagem de erro se "VALUES" não for encontrado após "INTO nomeTabela"
                Console.WriteLine($"Syntax error: Parentheses of the columns not found.");
                return false;
            }
            // Se os nomes das colunas não estiverem presentes, verifica se há a inicialização ( de valores
            else if (!sqlWithNoSpaces[sqlWithNoSpaces.IndexOf("VALUES") + 6].ToString().Equals("("))
            {
                // Exibe mensagem de erro se os parênteses de valores não forem encontrados
                Console.WriteLine($"Syntax error: Parentheses not found.");
                return false;
            }

            // Verifica se o parêntese de valores foi fechado
            string sqlWithNoSpacesAndSemicolon = sqlWithNoSpaces.Replace(";", "");
            if (!sqlWithNoSpacesAndSemicolon[^1].Equals(')'))
            {
                // Exibe mensagem de erro se os parênteses não forem fechados
                Console.WriteLine($"Syntax error: Parentheses not found.");
                return false;
            }

            string typeOfQuotation;

            // Verifica qual aspas é usado como principal
            if (sql.IndexOf("'") != -1 || sql.IndexOf('"') != -1)
            {
                typeOfQuotation = sql.IndexOf("'") > sql.IndexOf('"') ? "simple" : "double";
            }
            // Se não houver nenhuma aspas
            else
            {
                typeOfQuotation = "none";
            }

            // Verifica os valores dentro dos parênteses. Remove o primeiro parênteses
            string insertValues = sql.Substring(sql.IndexOf("VALUES") + 6).Substring(1);
            insertValues = insertValues.Substring(0, insertValues.Length - 1);

            string commasAndQuotations;
            int numOfQuotations = 0;
            switch (typeOfQuotation)
            {
                case "simple":
                    commasAndQuotations = new string(insertValues.Where(c => c == '\'' || c == ',').ToArray());
                    numOfQuotations = commasAndQuotations.Count(c => c == '\''); break;
                case "double":
                    commasAndQuotations = new string(insertValues.Where(c => c == '\"' || c == ',').ToArray());
                    numOfQuotations = commasAndQuotations.Count(c => c == '\"'); break;
            }

            // Exibe a quantidade de colunas e os valores dentro dos parênteses (sem as aspas)
            if(numOfCols > 0 && typeOfQuotation != "none")
            {
                // Exibe mensagem de erro se algum parênteses das colunas não foi fechado
                Console.WriteLine($"Syntax error: Non-closed parentheses.");
                return false;
            }

            // FALTA VERIFICA QUANTIDADE DE COLUNAS E VÍRGULAS
            Console.WriteLine(numOfCols);
            Console.WriteLine(insertValues);

            // Aqui devem ser adicionadas as outras regras de validação para a instrução INSERT/DELETE/UPDATE...

            return true;
        }
    }
}
