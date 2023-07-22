namespace SqlChecker;

public class CheckDml
{
    // Método para detectar qual tipo do comando DML e verificar se a instrução é válida
    public static bool IsValid(string dmlCommand)
    {
        bool result;
        // Remove os espaços laterais
        dmlCommand = dmlCommand.Trim();
        // Coleta o primeiro comando na string
        string command = dmlCommand.Split(" ")[0].ToUpper();
        // Define a cor de destaque para mensagens de erro
        Console.ForegroundColor = ConsoleColor.Red;
        // Faz a chamada correta para o comando
        switch (command)
        {
            case "INSERT":

                result = CheckInsert.IsInsertValid(dmlCommand);
                break;
            case "DELETE":
                result = false;
                break;
            case "UPDATE":
                result = false;
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

    // Método para verificar se o comando Insert é válido
    public static bool IsInsertValid(string insertCommand)
    {
        return CheckInsert.IsInsertValid(insertCommand);
    }
}