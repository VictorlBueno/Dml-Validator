using System.Text.RegularExpressions;
namespace SqlChecker;

internal class CheckInsert
{
    // Método para verificar se uma instrução INSERT/DELETE/UPDATE é válida
    internal static bool IsInsertValid(string insertCommand)
    {
        // Transforma em lista e remove espaços
        List<string> sqlAsList = insertCommand.Split(" ").ToList();
        sqlAsList.RemoveAll(" ".Contains);

        // INSERT\s*INTO\s*\w*
        // VALIDA INSERT INTO E NOME DA TABELA

        Dictionary<string, bool> syntaxRules = new Dictionary<string, bool>
            {
                { "hasColumnName", (sqlAsList[2].Contains("(") || sqlAsList[3].Contains("(")) },
                { "hasParenthesisNextToColumn", sqlAsList[2].Contains("(") },
                { "hasParenthesisBehindOfValues", insertCommand.Contains(")VALUES") },
                { "hasParenthesisFrontOfValues", insertCommand.Contains("VALUES(") },
                { "hasSemicolon", insertCommand[insertCommand.Length-1].ToString().Equals(";") },
            };

        // { "INSERT", 1 OU MAIS ESPAÇOS, "INTO", 1 OU MAIS ESPAÇOS, NOME ALFANUMÉRICO, 0 OU MAIS ESPAÇOS }
        List<string> patternList = new List<string> { "INSERT", "\\s+", "INTO", "\\s+", "\\w+", "\\s*" };

        // Posições dos parenteses
        int numOfCols = 0;
        int openParenthesisAfterValuesIndex = insertCommand.IndexOf("(");
        int firstOpenParenthesisIndex = insertCommand.IndexOf("(");
        int firstCloseParenthesisIndex = insertCommand.IndexOf(")");

        // Se tiver os nomes das colunas
        if (syntaxRules["hasColumnName"])
        {

            openParenthesisAfterValuesIndex = insertCommand.IndexOf('(', firstOpenParenthesisIndex + 1);

            // Coleta apenas os nomes das colunas e conta quantas colunas existem
            string columnNames = insertCommand.Substring(firstOpenParenthesisIndex + 1, firstCloseParenthesisIndex - firstOpenParenthesisIndex - 1);
            numOfCols = columnNames.Split(",").Length;

            // Adicione o parênteses de abertura
            patternList.Add("\\(\\s*");

            // Adicione a quantidade de colunas esperadas
            for (int i = 0; i < numOfCols; i++)
            {
                // Adicione um nome de coluna alfanumérico e espaços adicionais
                patternList.Add("\\w+\\s*");

                // Adicione vírgula e espaços adicionais se houver mais colunas a frente
                if (i != numOfCols - 1)
                {
                    patternList.Add(",\\s*");
                }
            }

            // Adicione no pattern: parênteses de fechamento e espaço opcional
            patternList.Add("\\)\\s*");

            // Se não tiver os nomes das colunas, contar a quantidade de valores
        }
        else
        {
            // Coleta apenas os nomes das colunas e conta quantas colunas existem
            string columnNames = insertCommand.Substring(firstOpenParenthesisIndex + 1, firstCloseParenthesisIndex - firstOpenParenthesisIndex - 1);
            numOfCols = columnNames.Split(",").Length;
        }

        // Adicione no pattern: "VALUES", espaço opcional, abertura de parênteses e espaço opcional
        patternList.Add("VALUES\\s*\\(\\s*");

        // Verifica a aspas príncipal da sintaxe
        char primaryQuote = insertCommand[openParenthesisAfterValuesIndex + 1];
        string anyTextBetweenQuote = "(" + primaryQuote + ".*" + primaryQuote;

        // Se não houver aspas no comando, remova a opção
        if (!primaryQuote.Equals("'") || !primaryQuote.Equals("\""))
        {
            // Só permitir números
            anyTextBetweenQuote = "(\\d+(\\.\\d+)?";
        }

        // Adicione a quantidade de valores esperados de acordo com a quantidade de colunas
        for (int i = 0; i < numOfCols; i++)
        {
            // Adicione valores iguais a texto, números decimais ou números inteiros
            patternList.Add(anyTextBetweenQuote + "|\\d+(\\.\\d+)?)");

            // Adicione vírgula e espaços adicionais se houver mais colunas a frente
            if (i != numOfCols - 1)
            {
                patternList.Add(",\\s*");
            }
        }

        // Fecha os parênteses de valores e ponto e vírgula
        string semiColon = syntaxRules["hasSemicolon"] ? ";" : "";
        patternList.Add("\\)" + semiColon);

        // Transforma o Regex final e compara
        string finalPattern = string.Join("", patternList);
        Match match = Regex.Match(insertCommand, finalPattern);

        return match.Success;
    }
}
