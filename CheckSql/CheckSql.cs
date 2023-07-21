using System;
using System.Collections;
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
            // Remove os espaços laterais
            sql = sql.Trim();
            // Coleta o primeiro comando na string
            string command = sql.Split(" ")[0].ToUpper();
            // Define a cor de destaque para mensagens de erro
            Console.ForegroundColor = ConsoleColor.Red;
            // Faz a chamada correta para o comando
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
                    Console.WriteLine($"\"{command}\" is not recognized as a DML command.");
                    break;
            }

            // Reseta a cor da fonte para o padrão
            Console.ResetColor();
            return result;
        }

        // Método para verificar se uma instrução INSERT/DELETE/UPDATE é válida
        private static bool IsInsertValid(string sql)
        {
            // Transforma em lista e remove espaços
            List<string> sqlAsList = sql.Split(" ").ToList();
            sqlAsList.RemoveAll(" ".Contains);

            // INSERT\s*INTO\s*\w*
            // VALIDA INSERT INTO E NOME DA TABELA

            Dictionary<string, bool> syntaxRules = new Dictionary<string, bool>
            {
                { "hasColumnName", (sqlAsList[2].Contains("(") || sqlAsList[3].Contains("(")) },
                { "hasParenthesisNextToColumn", sqlAsList[2].Contains("(") },
                { "hasParenthesisBehindOfValues", sql.Contains(")VALUES") },
                { "hasParenthesisFrontOfValues", sql.Contains("VALUES(") },
                { "hasSemicolon", sql[sql.Length-1].ToString().Equals(";") },
            };

            // { "INSERT", 1 OU MAIS ESPAÇOS, "INTO", 1 OU MAIS ESPAÇOS, NOME ALFANUMÉRICO, 0 OU MAIS ESPAÇOS }
            List<string> patternList = new List<string> { "INSERT", "\\s+", "INTO", "\\s+", "\\w+", "\\s*" };

            // Posições dos parenteses
            int numOfCols = 0;
            int openParenthesisAfterValuesIndex = sql.IndexOf("(");
            int firstOpenParenthesisIndex = sql.IndexOf("(");
            int firstCloseParenthesisIndex = sql.IndexOf(")");

            // Se tiver os nomes das colunas
            if (syntaxRules["hasColumnName"])
            {
                
                openParenthesisAfterValuesIndex = sql.IndexOf('(', firstOpenParenthesisIndex + 1);

                // Coleta apenas os nomes das colunas e conta quantas colunas existem
                string columnNames = sql.Substring(firstOpenParenthesisIndex + 1, firstCloseParenthesisIndex - firstOpenParenthesisIndex - 1);
                numOfCols = columnNames.Split(",").Length;

                // Adicione o parênteses de abertura
                patternList.Add("\\(");

                // Adicione a quantidade de colunas esperadas
                for (int i = 0; i < numOfCols; i++)
                {
                    // Adicione um nome de coluna alfanumérico e espaços adicionais
                    patternList.Add("\\w+\\s*");

                    // Adicione vírgula e espaços adicionais se houver mais colunas a frente
                    if(i != numOfCols - 1)
                    {
                        patternList.Add(",\\s*");
                    }
                }

                // Adicione no pattern: parênteses de fechamento e espaço opcional
                patternList.Add("\\)\\s*");

            // Se não tiver os nomes das colunas, contar a quantidade de valores
            } else
            {
                // Coleta apenas os nomes das colunas e conta quantas colunas existem
                string columnNames = sql.Substring(firstOpenParenthesisIndex + 1, firstCloseParenthesisIndex - firstOpenParenthesisIndex - 1);
                numOfCols = columnNames.Split(",").Length;
            }

            // Adicione no pattern: "VALUES", espaço opcional, abertura de parênteses e espaço opcional
            patternList.Add("VALUES\\s*\\(*\\s*");

            // Verifica a aspas príncipal da sintaxe
            char primaryQuote = sql[openParenthesisAfterValuesIndex + 1];

            // Adicione a quantidade de valores esperados de acordo com a quantidade de colunas
            for (int i = 0; i < numOfCols; i++)
            {
                // Adicione um nome de coluna alfanumérico e espaços adicionais
                patternList.Add("(" + primaryQuote + ".*" + primaryQuote + "|[\\w.]+)");

                // Adicione vírgula e espaços adicionais se houver mais colunas a frente
                if (i != numOfCols - 1)
                {
                    patternList.Add(",\\s*");
                }
            }

            // Fecha os parênteses de valores e ponto e vírgula
            string semiColon = syntaxRules["hasSemicolon"] ? ";" : "";
            patternList.Add("\\)" + semiColon);
            

            string finalPattern = string.Join("", patternList);
            Match match = Regex.Match(sql, finalPattern);

            Console.WriteLine(sql);
            Console.WriteLine("->" + finalPattern);


            return match.Success;
        }
    }
}
