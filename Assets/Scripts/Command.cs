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
        ["Light"] = new Light(),
        ["Tilemap"] = new TilemapCommand(),
        ["Help"]=new Help()
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
            return commandKeyWord[args[0]].Run(command.Split(' '));
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
    List<string> secondKeyWord { get; }
    CommandRunResult Run(string[] args);
}
class Lua : ICommand
{
    public string Name { get; } = "Lua";
    public List<string> secondKeyWord { get; } = new List<string>() { "mode" };
    public CommandRunResult Run(string[] args)
    {
        if (secondKeyWord.Contains(args[1])){
            
        }
        else
        {
            return new CommandRunResult(false, 1, $"{Name} 内没有 {args[1]} 这个二级函数");
        }
        return new CommandRunResult(true,null,"");
    }
}
class Help : ICommand
{
    public string Name { get; } = "Help";
    public List<string> secondKeyWord { get; } = new List<string>() { null };
    public CommandRunResult Run(string[] args)
    {
        return new CommandRunResult(true, resultMessage: @"Light [bool] : 是否全局接受2D光照
Tilemap speed [int] : 设置生成多少方格更新一帧
Tilemap collider [bool] : 设置生成方格时是否取消碰撞体碰撞
");
    }

}
class Light : ICommand
{
    public string Name { get; } = "Light";
    public List<string> secondKeyWord { get; } = new List<string>();
    public CommandRunResult Run(string[] args)
    {
        var material = ((GameObject)Command.s_commandData[0]).GetComponent<TilemapRenderer>();
        if (args[1] == "true")
        {
            material.material = (Material)Command.s_commandData[1];
        }
        else if (args[1] == "false")
        {
            material.material = (Material)Command.s_commandData[2];
        }
        else
        {
            return new CommandRunResult(false, 10, "只能是true或者false");
        }
        return new CommandRunResult(true);
    }
}
class TilemapCommand : ICommand
{
    public string Name { get; } = "Tilemap";
    public List<string> secondKeyWord { get; } = new List<string>() { "speed","collider" };
    public CommandRunResult Run(string[] args)
    {
        TilemapSpawn tilemapSpawn = ((GameObject)Command.s_commandData[3]).GetComponent<TilemapSpawn>();
        switch (args[1])
        {
            case "speed":
                {
                    if (int.TryParse(args[2], out int result))
                    {
                        tilemapSpawn.spawnMapSpeed = result;
                        return new CommandRunResult(true);
                    }
                    else
                    {
                        return new CommandRunResult(false, 11, $"{args[2]} 不是数字");
                    }
                }
            case "collider":
                {
                    if (args[2] == "true") { tilemapSpawn.isUpdateColliderOnFrame = true; return new CommandRunResult(true); }
                    else if (args[2] == "false") { tilemapSpawn.isUpdateColliderOnFrame = false; return new CommandRunResult(true); }
                    else
                    {
                        return new CommandRunResult(false, 10, "只能是true或者false");
                    }
                }
            default:
                {
                    return new CommandRunResult(false, 1, $"{Name} 内没有 {args[1]} 这个二级函数");
                }
        }
        
    }
}
public struct CommandRunResult
{
    public CommandRunResult(bool isCorrect,int? errorCode=null,string errorMessage=null, string resultMessage = "")
    {
        this.isCorrect = isCorrect;
        this.errorCode = errorCode;
        this.errorMessage = errorMessage;
        this.resultMessage = resultMessage;
    }
    public readonly string resultMessage;
    public readonly bool isCorrect;
    public readonly int? errorCode;
    public readonly string errorMessage;
}