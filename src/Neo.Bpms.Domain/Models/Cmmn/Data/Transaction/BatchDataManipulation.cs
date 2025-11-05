using System.Text;

namespace Neo.Bpms.Domain.Models.Cmmn.Data.Transaction;

public class BatchDataManipulation(IBatchDataManipulationRunner runner)
{
    public bool Batch { get; set; }
    public StringBuilder Commands { get; set; }
    private readonly IBatchDataManipulationRunner _runner = runner;
    private int _batchCount;
    private int _batchCounter = 0;
    private readonly object _loc = new();

    public void BeginBatch(int batchCount = 0)
    {
        lock (_loc)
        {
            Batch = true;
            _batchCount = batchCount;
            _batchCounter = 0;
            if (Commands != null && Commands.Length > 0)
                RunCommands();
        }
    }

    public void EndBatch()
    {
        lock (_loc)
        {
            if (!Batch) return;
            RunCommands();
            Batch = false;
            Release();
        }
    }

    public void Append(string command)
    {
        lock (_loc)
        {
            if (Commands == null)
            {
                Commands = new StringBuilder(command);
                return;
            }

            if (Commands.Length > 0)
            {
                if (Commands[Commands.Length - 1] != ';')
                    Commands.Append(';');
            }

            command = command.TrimEnd(' ', '\n', '\r', '\t');
            Commands.Append(command);
            _batchCounter++;
        }
    }

    public int CheckAndRunCommands()
    {
        lock (_loc)
        {
            if (Commands == null)
                return 0;
            if (Batch)
            {
                if (Commands.Length >= 4000000 || _batchCount > 0 && _batchCounter >= _batchCount)
                    return RunCommands();
            }
            else
                return RunCommands();

            return 0;
        }
    }

    public int Commit()
    {
        lock (_loc)
        {
            int recordsAffected = RunCommands();
            Release();
            return recordsAffected;
        }
    }

    private int RunCommands()
    {
        _batchCounter = 0;
        if (Commands == null || Commands.Length == 0)
            return 0;
        string sqlCommand = Commands.ToString();
        Commands.Clear();
        return RunCommand(sqlCommand);
    }

    private int RunCommand(string sqlCommand)
    {
        return _runner?.RunCommand(sqlCommand) ?? 0;
    }

    private void Release()
    {
        lock (_loc)
        {
            _runner?.Release();
        }
    }
}