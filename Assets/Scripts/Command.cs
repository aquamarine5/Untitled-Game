using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Command : MonoBehaviour
{
    public static Dictionary<int, Object> commandDictData = new Dictionary<int, Object>();
    public List<Object> commandData = new List<Object>();
    public static List<Object> s_commandData = new List<Object>();
    public static Dictionary<string, ICommand> commandKeyWord = new Dictionary<string, ICommand>() {
        ["Lua"] = new Lua(),
        ["Light"] = new Light()
    };
    private void Awake()
    {
        s_commandData = commandData;
    }
    public static CommandRunResult RunCommand(string command)
    {
        string[] args = command.Split(' ');
        if (commandKeyWord.ContainsKey(args[0]))
        {
            return commandKeyWord[args[0]].Run(command);
        }
        else
        {
            return new CommandRunResult(false, 0, "没有找到函数");
        }
    }
}
public interface ICommand
{
    string Name { get; }
    CommandRunResult Run(string command);
}
class Lua : ICommand
{
    public string Name { get; } = "Lua";
    List<string> secondKeyWord = new List<string>() { "mode" };
    public CommandRunResult Run(string command)
    {
        string[] args = command.Split(' ');
        if (secondKeyWord.Contains(args[1])){
            
        }
        else
        {
            return new CommandRunResult(false, 1, $"{Name} 内没有 {args[1]} 这个二级函数");
        }
        return new CommandRunResult(true,null,"");
    }
}
class Light : ICommand
{
    public string Name { get; } = "Light";
    List<string> secondKeyWord = new List<string>();
    public CommandRunResult Run(string command)
    {
        string[] args = command.Split(' ');
        if (args[1] == "true")
        {
            ((GameObject)Command.s_commandData[0]).GetComponent<TilemapRenderer>().material = (Material)Command.s_commandData[1];
        }
        else if (args[1] == "false")
        {
            ((GameObject)Command.s_commandData[0]).GetComponent<TilemapRenderer>().material = (Material)Command.s_commandData[2];
        }
        else
        {
            return new CommandRunResult(false, 10, "只能是true或者false");
        }
        return new CommandRunResult(true, null, null);
    }
}
public struct CommandRunResult
{
    public CommandRunResult(bool isCorrect,int? errorCode,string errorMessage)
    {
        this.isCorrect = isCorrect;
        this.errorCode = errorCode;
        this.errorMessage = errorMessage;
    }
    public readonly bool isCorrect;
    public readonly int? errorCode;
    public readonly string errorMessage;
}