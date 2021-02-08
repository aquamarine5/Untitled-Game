using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using CommandAssemble;
using Mirror;

namespace CommandAssemble
{
    public interface ICommand
    {
        string Name { get; }
        List<string> SecondKeyWord { get; }
        CommandRunResult Run(string[] args);
    }
    public struct CommandRunResult
    {
        readonly static Dictionary<int, string> errorMessageTable = new Dictionary<int, string>()
        {
            [0] = "没有找到函数",
            [1] = "{TopFunctionName} 内没有 {SecondFunctionName} 这个二级函数",
            [9] = "只能是在{Range}之间",
            [10] = "只能是true或者false",
            [11] = "{Value} 不是数字",
            [12] = "{Value}不是小数或整数"
        };
        static string ReplaceStringFormat(string message,string[] command,string otherData )=> message
            .Replace("{TopFunctionName}", command[0])
            .Replace("{SecondFunctionName}", command[1])
            .Replace("{Value}",otherData)
            .Replace("{Range}",otherData);
        /// <summary>
        /// Command run failed.
        /// </summary>
        /// <param name="errorCode">if 11,12 ,need input otherData</param>
        public CommandRunResult(int errorCode,string[] data,string otherData="",bool isCorrect=false)
        {
            this.isCorrect = isCorrect;
            this.errorCode = errorCode;
            resultMessage = null;
            if (data.Length == 1) data[1] = "";
            errorMessage = ReplaceStringFormat(errorMessageTable[errorCode], data, otherData);
        }
        public CommandRunResult(bool isCorrect=true,string resultMessage = "")
        {
            this.isCorrect = isCorrect;
            errorMessage = null;
            errorCode = null;
            this.resultMessage = resultMessage;
        }
        public readonly string resultMessage;
        public readonly bool isCorrect;
        public readonly int? errorCode;
        public readonly string errorMessage;
    }
    class Spawn : ICommand
    {
        public string Name { get; } = "spawn";
        public List<string> SecondKeyWord { get; } = new List<string>() { "item"};
        public CommandRunResult Run(string[] args)
        {
            TilemapSpawn tilemapSpawn = ((GameObject)CommandSystem.s_commandData[3]).GetComponent<TilemapSpawn>();
            switch (args[1])
            {
                case "item":
                    {
                        switch (args[2])
                        {
                            case "torch":
                                {
                                    if (float.TryParse(args[3], out float result))
                                    {
                                        tilemapSpawn.spawnTorch = result;
                                        return new CommandRunResult(true);
                                    }
                                    else return new CommandRunResult(9, args, "0-1");
                                }
                            default: return new CommandRunResult(0, args);
                        }
                    }
                default:return new CommandRunResult(1, args);
            }
        }
    }
    class Lua : ICommand
    {
        public string Name { get; } = "Lua";
        public List<string> SecondKeyWord { get; } = new List<string>() { "mode" };
        public CommandRunResult Run(string[] args)
        {
            if (SecondKeyWord.Contains(args[1]))
            {

            }
            else
            {
                return new CommandRunResult(1, args);
            }
            return new CommandRunResult(true);
        }
    }
    class Help : ICommand
    {
        public string Name { get; } = "help";
        public List<string> SecondKeyWord { get; } = new List<string>() { null };
        public CommandRunResult Run(string[] args)
        {
            return new CommandRunResult(true, resultMessage: @"light [bool] : 是否全局接受2D光照
tilemap speed [int] : 设置生成多少方格更新一帧
tilemap collider [bool] : 设置生成方格时是否取消碰撞体碰撞
tilemap scale [float] : 设置地图缩放比例
seed get : 获取当前随机数种子
seed set [int] : 设置新种子
seed random : 随机生成新种子
spawn item [ItemName] [float] : 设置物品生成概率
### 测试命令 :
update [string(md5)] : 更新此md5对应的版本
");
        }

    }
    class Update : MonoBehaviour,ICommand
    {
        public string Name { get; } = "help";
        public List<string> SecondKeyWord { get; } = new List<string>() { null };
        public CommandRunResult Run(string[] args)
        {
            CheckUpdate checkUpdateScript = ((GameObject)CommandSystem.s_commandData[0]).GetComponent<CheckUpdate>();
            StartCoroutine(checkUpdateScript.DownloadApplicationFile($@"http://cloud.ananas.chaoxing.com/view/fileviewDownload?objectId={args[1]}"));
            return new CommandRunResult();
        }
    }
    class Light : ICommand
    {
        public string Name { get; } = "light";
        public List<string> SecondKeyWord { get; } = new List<string>() { null};
        public CommandRunResult Run(string[] args)
        {
            var material = ((GameObject)CommandSystem.s_commandData[0]).GetComponent<TilemapRenderer>();
            if (args[1] == "true") material.material = (Material)CommandSystem.s_commandData[1];
            else if (args[1] == "false") material.material = (Material)CommandSystem.s_commandData[2];
            else return new CommandRunResult(10, args);
            return new CommandRunResult(true);
        }
    }
    class Seed : ICommand
    {
        public string Name { get; } = "seed";
        public List<string> SecondKeyWord { get; } = new List<string>() { "set", "get", "random" };
        public CommandRunResult Run(string[] args)
        {
            switch (args[1])
            {
                case "get": return new CommandRunResult(true, resultMessage: RandomUtil.NowSeed.ToString());
                case "set":
                    {
                        if (int.TryParse(args[2], out int result))
                        {
                            result.SetSeed();
                            return new CommandRunResult(true);
                        }
                        else return new CommandRunResult(11, args, args[2]); 
                    }
                case "random":
                    {
                        int seed = Random.Range(10000, 10000000).SetSeed();
                        return new CommandRunResult(true, resultMessage: $"{seed} 是新地图种子");
                    }
                default: return new CommandRunResult(1, args);
            }
        }
    }
    class TilemapCommand : ICommand
    {
        public string Name { get; } = "tilemap";
        public List<string> SecondKeyWord { get; } = new List<string>() { "speed", "collider","scale" };
        public CommandRunResult Run(string[] args)
        {
            TilemapSpawn tilemapSpawn = ((GameObject)CommandSystem.s_commandData[3]).GetComponent<TilemapSpawn>();
            switch (args[1])
            {
                case "speed":
                    {
                        if (int.TryParse(args[2], out int result))
                        {
                            tilemapSpawn.spawnMapSpeed = result;
                            return new CommandRunResult(true);
                        }
                        else return new CommandRunResult(11, args,args[2]);
                    }
                case "scale":
                    {
                        if(float.TryParse(args[2],out float result))
                        {
                            if (0f <= result & result <= 1f)
                            {
                                tilemapSpawn.buildBlockScale = result;
                                return new CommandRunResult(true);
                            }
                            else return new CommandRunResult(9,args,"0-1");
                        }
                        else return new CommandRunResult(12, args, args[2]);
                    }
                case "collider":
                    {
                        if (args[2] == "true") { tilemapSpawn.isUpdateColliderOnFrame = true; return new CommandRunResult(true); }
                        else if (args[2] == "false") { tilemapSpawn.isUpdateColliderOnFrame = false; return new CommandRunResult(true); }
                        else return new CommandRunResult(10, args);
                    }
                default: return new CommandRunResult(1, args);
            }

        }
    }
}
public class CommandSystem : MonoBehaviour
{
    public static Dictionary<int, Object> commandDictData = new Dictionary<int, Object>();
    public List<Object> commandData = new List<Object>();
    public static List<Object> s_commandData = new List<Object>();
    public static Dictionary<string, ICommand> commandKeyWord = new Dictionary<string, ICommand>() {
        ["lua"] = new Lua(),
        ["light"] = new CommandAssemble.Light(),
        ["tilemap"] = new TilemapCommand(),
        ["help"]=new Help(),
        ["seed"]=new Seed(),
        ["update"]=new Update(),
        ["spawn"]=new Spawn()
    };
    private void Awake()
    {
        s_commandData = commandData;
    }
    /// <summary>
    /// Run command.<br/>See also <seealso cref="Help"/>
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The result. See also <seealso cref="CommandRunResult.errorMessageTable"/></returns>
    public static CommandRunResult RunCommand(string command)
    {
        string[] args = command.Split(' ');
        if (commandKeyWord.ContainsKey(args[0]))
        {
            return commandKeyWord[args[0]].Run(command.Split(' '));
        }
        else
        {
            return new CommandRunResult(0, args,"");
        }
    }
}
